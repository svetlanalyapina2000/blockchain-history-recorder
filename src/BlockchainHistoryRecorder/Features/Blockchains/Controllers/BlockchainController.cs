using BlockchainHistoryRecorder.Features.Blockchains.Application.Commands.StoreBlockchain;
using BlockchainHistoryRecorder.Features.Blockchains.Application.Queries.GetBlockchain;
using BlockchainHistoryRecorder.Features.Blockchains.Application.Queries.GetBlockchainHistory;
using BlockchainHistoryRecorder.Features.Blockchains.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlockchainHistoryRecorder.Features.Blockchains.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlockchainController : ControllerBase
{
    private readonly IMediator _mediator;

    public BlockchainController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    ///     Fetches the current state of a blockchain and stores it.
    /// </summary>
    /// <param name="blockchainIdentifier">The identifier of the blockchain to fetch and store.</param>
    /// <returns>A response containing the stored blockchain data.</returns>
    /// <response code="200">Returns the stored blockchain response</response>
    /// <response code="400">If the blockchainIdentifier is invalid or not supported</response>
    /// <response code="500">If there is an internal server error while processing the request</response>
    [HttpPost]
    [ProducesResponseType(typeof(StoreBlockchainResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<StoreBlockchainResponse> FetchBlockchain(BlockchainIdentifier blockchainIdentifier)
    {
        var response = await _mediator.Send(new GetBlockchainQuery { BlockchainIdentifier = blockchainIdentifier });
        return await _mediator.Send(new StoreBlockchainCommand { Blockchain = response.Blockchain });
    }

    /// <summary>
    ///     Retrieves the history of a specified blockchain records.
    /// </summary>
    /// <param name="blockchainIdentifier">The identifier of the blockchain to retrieve history for.</param>
    /// <returns>The blockchain history response.</returns>
    /// <response code="200">Returns the blockchain history</response>
    /// <response code="400">If the blockchainIdentifier is invalid or not supported</response>
    /// <response code="500">If there is an internal server error while processing the request</response>
    [HttpGet("history")]
    [ProducesResponseType(typeof(GetBlockchainHistoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<GetBlockchainHistoryResponse> GetBlockchainHistory(BlockchainIdentifier blockchainIdentifier)
    {
        var blockchainHistory = await _mediator.Send(new GetBlockchainHistoryQuery
            { BlockchainIdentifier = blockchainIdentifier });
        return blockchainHistory;
    }
}