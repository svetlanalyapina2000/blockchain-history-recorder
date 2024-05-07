using System.Net;
using System.Net.Http.Json;
using BlockchainHistoryRecorder.Features.Blockchains.Application.Commands.StoreBlockchain;
using BlockchainHistoryRecorder.Features.Blockchains.Application.Queries.GetBlockchainHistory;
using BlockchainHistoryRecorder.Features.Blockchains.Domain;
using BlockchainHistoryRecorder.Tests.Features.Blockchain.TestFactories;
using BlockchainHistoryRecorder.Tests.Functional.Abstractions;
using BlockchainHistoryRecorder.Tests.Functional.Features.Blockchain.TestData;
using FluentAssertions;

namespace BlockchainHistoryRecorder.Tests.Functional.Features.Blockchain;

public class GetBlockchainHistoryControllerTests : BaseTests
{
    #region Entities

    private readonly List<BlockchainHistoryRecorder.Features.Blockchains.Domain.Blockchain> _blockchains = [];

    #endregion

    public GetBlockchainHistoryControllerTests(TestWebApplicationFactory factory) : base(factory)
    {
        _blockchains.Add(BlockchainTestFactory.CreateBlockchainRecord(Fixture, CassandraMapper,
            BlockchainIdentifier.BitcoinMainnet));
        _blockchains.Add(BlockchainTestFactory.CreateBlockchainRecord(Fixture, CassandraMapper,
            BlockchainIdentifier.BitcoinTestnet));
        _blockchains.Add(
            BlockchainTestFactory.CreateBlockchainRecord(Fixture, CassandraMapper, BlockchainIdentifier.DashMainnet));
        _blockchains.Add(BlockchainTestFactory.CreateBlockchainRecord(Fixture, CassandraMapper,
            BlockchainIdentifier.LitecoinMainnet));
        _blockchains.Add(BlockchainTestFactory.CreateBlockchainRecord(Fixture, CassandraMapper,
            BlockchainIdentifier.EthereumMainnet));
    }

    [Theory]
    [MemberData(nameof(BlockchainIdentifierTestData.BlockchainIdentifiers),
        MemberType = typeof(BlockchainIdentifierTestData))]
    public async Task FetchBlockchain_NormalFlow(BlockchainIdentifier blockchainIdentifier)
    {
        var response =
            await HttpClient.PostAsync($"api/blockchain?blockchainIdentifier={blockchainIdentifier.ToString()}", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var blockchain = await response.Content.ReadFromJsonAsync<StoreBlockchainResponse>();
        blockchain!.Name.Should().NotBeEmpty();
        //Cleanup
        BlockchainTestFactory.DeleteBlockchainRecord(Session, blockchain.Name, blockchain.CreatedAt);
    }


    [Theory]
    [MemberData(nameof(BlockchainIdentifierTestData.BlockchainIdentifiers),
        MemberType = typeof(BlockchainIdentifierTestData))]
    public async Task GetBlockchainHistory_NormalFlow(BlockchainIdentifier blockchainIdentifier)
    {
        var response =
            await HttpClient.GetAsync($"api/blockchain/history?blockchainIdentifier={blockchainIdentifier.ToString()}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var blockchains = await response.Content.ReadFromJsonAsync<GetBlockchainHistoryResponse>();
        blockchains?.BlockchainHistory.Should().HaveCount(1);
        var blockchain = blockchains!.BlockchainHistory.First();
        var testBlockchain = _blockchains.First(x => x.Name == blockchain.Name);
        blockchain.Name.Should().Be(testBlockchain.Name);
        blockchain.Hash.Should().Be(testBlockchain.Hash);
        blockchain.Height.Should().Be(testBlockchain.Height);
        blockchain.Time.Should().BeCloseTo(testBlockchain.Time, TimeSpan.FromMilliseconds(1));
        blockchain.LatestUrl.Should().Be(testBlockchain.LatestUrl);
        blockchain.PreviousHash.Should().Be(testBlockchain.PreviousHash);
        blockchain.PreviousUrl.Should().Be(testBlockchain.PreviousUrl);
        blockchain.PeerCount.Should().Be(testBlockchain.PeerCount);
        blockchain.UnconfirmedCount.Should().Be(testBlockchain.UnconfirmedCount);
        blockchain.HighFeePerKb.Should().Be(testBlockchain.HighFeePerKb);
        blockchain.MediumFeePerKb.Should().Be(testBlockchain.MediumFeePerKb);
        blockchain.LowFeePerKb.Should().Be(testBlockchain.LowFeePerKb);
        blockchain.HighGasPrice.Should().Be(testBlockchain.HighGasPrice);
        blockchain.MediumGasPrice.Should().Be(testBlockchain.MediumGasPrice);
        blockchain.LowGasPrice.Should().Be(testBlockchain.LowGasPrice);
        blockchain.LastForkHeight.Should().Be(testBlockchain.LastForkHeight);
        blockchain.LastForkHash.Should().Be(testBlockchain.LastForkHash);
        blockchain.CreatedAt.Should().BeCloseTo(testBlockchain.CreatedAt, TimeSpan.FromMilliseconds(1));
    }


    [Fact]
    public async Task FetchBlockchain_UnknownBlockchainIdentifier_ReturnBadRequest()
    {
        var response =
            await HttpClient.PostAsync($"api/blockchain?blockchainIdentifier={BlockchainIdentifier.Unknown.ToString()}",
                null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetBlockchainHistory_UnknownBlockchainIdentifier_ReturnError()
    {
        var response =
            await HttpClient.GetAsync(
                $"api/blockchain/history?blockchainIdentifier={BlockchainIdentifier.Unknown.ToString()}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task FetchBlockchain_ConcurrentRequests()
    {
        var tasks = Enumerable.Range(0, 10)
            .Select(_ =>
                HttpClient.GetAsync(
                    $"api/blockchain/history?blockchainIdentifier={BlockchainIdentifier.BitcoinMainnet}"))
            .ToList();

        var responses = await Task.WhenAll(tasks);

        responses.Should().OnlyContain(r => r.StatusCode == HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task FetchBlockchain_InvalidIdentifier_ReturnBadRequest(string identifier)
    {
        var response = await HttpClient.PostAsync($"api/blockchain?blockchainIdentifier={identifier}", null);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    protected override void Cleanup()
    {
        foreach (var blockchain in _blockchains)
            BlockchainTestFactory.DeleteBlockchainRecord(Session, blockchain.Name, blockchain.CreatedAt);
    }
}