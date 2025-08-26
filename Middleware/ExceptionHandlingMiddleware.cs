// Middleware/ErrorHandlingMiddleware.cs

// Middleware to handle exceptions globally in the application
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    // Constructor to initialize the middleware with the next delegate in the pipeline
    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    // Middleware logic to catch and handle exceptions
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Pass the request to the next middleware in the pipeline
            await _next(context);
        }
        catch (Exception ex)
        {
            // Handle any unhandled exceptions that occur in the pipeline
            context.Response.StatusCode = 500; // Set HTTP status code to 500 (Internal Server Error)
            context.Response.ContentType = "application/json"; // Set response content type to JSON

            // Create an error response object
            var errorResponse = new
            {
                StatusCode = context.Response.StatusCode, // Include the status code
                Message = "Something went wrong!", // Generic error message
                Details = ex.Message // Include exception details (optional: hide in production)
            };

            // Write the error response as JSON to the HTTP response
            await context.Response.WriteAsync(
                System.Text.Json.JsonSerializer.Serialize(errorResponse));
        }
    }
}
