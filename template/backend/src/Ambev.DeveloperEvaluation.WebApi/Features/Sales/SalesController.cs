using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

[ApiController]
[Route("api/[controller]")]
public class SalesController : BaseController
{
    private readonly IMediator _mediator;

    public SalesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<SaleResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSale(
        [FromBody] CreateSaleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateSaleCommand
        {
            SaleNumber = request.SaleNumber,
            SaleDate = request.SaleDate,
            CustomerId = request.CustomerId,
            CustomerName = request.CustomerName,
            BranchId = request.BranchId,
            BranchName = request.BranchName,
            Items = request.Items.Select(item => new SaleItemInput
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };

        var response = await _mediator.Send(command, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, new ApiResponseWithData<SaleResult>
        {
            Success = true,
            Message = "Sale created successfully",
            Data = response
        });
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<IReadOnlyCollection<SaleResult>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListSales(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new ListSalesCommand(), cancellationToken);

        return StatusCode(StatusCodes.Status200OK, new ApiResponseWithData<IReadOnlyCollection<SaleResult>>
        {
            Success = true,
            Message = "Sales retrieved successfully",
            Data = response
        });
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseWithData<SaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSale(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetSaleCommand(id), cancellationToken);

        return StatusCode(StatusCodes.Status200OK, new ApiResponseWithData<SaleResult>
        {
            Success = true,
            Message = "Sale retrieved successfully",
            Data = response
        });
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseWithData<SaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSale(
        [FromRoute] Guid id,
        [FromBody] UpdateSaleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateSaleCommand
        {
            Id = id,
            SaleNumber = request.SaleNumber,
            SaleDate = request.SaleDate,
            CustomerId = request.CustomerId,
            CustomerName = request.CustomerName,
            BranchId = request.BranchId,
            BranchName = request.BranchName,
            Items = request.Items.Select(item => new SaleItemInput
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };

        var response = await _mediator.Send(command, cancellationToken);

        return StatusCode(StatusCodes.Status200OK, new ApiResponseWithData<SaleResult>
        {
            Success = true,
            Message = "Sale updated successfully",
            Data = response
        });
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSale(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteSaleCommand(id), cancellationToken);

        return StatusCode(StatusCodes.Status200OK, new ApiResponse
        {
            Success = true,
            Message = "Sale deleted successfully"
        });
    }

    [HttpPatch("{id:guid}/cancel")]
    [ProducesResponseType(typeof(ApiResponseWithData<SaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelSale(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CancelSaleCommand(id), cancellationToken);

        return StatusCode(StatusCodes.Status200OK, new ApiResponseWithData<SaleResult>
        {
            Success = true,
            Message = "Sale cancelled successfully",
            Data = response
        });
    }

    [HttpPatch("{saleId:guid}/items/{itemId:guid}/cancel")]
    [ProducesResponseType(typeof(ApiResponseWithData<SaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelItem(
        [FromRoute] Guid saleId,
        [FromRoute] Guid itemId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CancelSaleItemCommand(saleId, itemId), cancellationToken);

        return StatusCode(StatusCodes.Status200OK, new ApiResponseWithData<SaleResult>
        {
            Success = true,
            Message = "Sale item cancelled successfully",
            Data = response
        });
    }
}