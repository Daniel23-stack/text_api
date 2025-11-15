using MediatR;
using Microsoft.AspNetCore.Mvc;
using NumberWordAnalyzer.Application.Commands;
using NumberWordAnalyzer.Api.Models;

namespace NumberWordAnalyzer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NumberWordAnalyzerController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<NumberWordAnalyzerController> _logger;

    public NumberWordAnalyzerController(IMediator mediator, ILogger<NumberWordAnalyzerController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Analyzes the input text and returns the count of each number word found.
    /// </summary>
    /// <param name="request">The request containing the input text to analyze</param>
    /// <returns>A dictionary with number word counts</returns>
    [HttpPost]
    [ProducesResponseType(typeof(AnalysisResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AnalysisResponse>> Analyze([FromBody] AnalysisRequest request)
    {
        _logger.LogInformation("Received analysis request for text of length {Length}", request.InputText?.Length ?? 0);

        var command = new AnalyzeNumberWordsCommand
        {
            InputText = request.InputText ?? string.Empty
        };

        var result = await _mediator.Send(command);

        var response = new AnalysisResponse
        {
            WordCounts = result.WordCounts
        };

        _logger.LogInformation("Analysis completed. Found {TotalCount} total occurrences", 
            result.WordCounts.Values.Sum());

        return Ok(response);
    }
}

