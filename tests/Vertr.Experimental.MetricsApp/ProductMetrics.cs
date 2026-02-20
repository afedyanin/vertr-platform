using System.Diagnostics.Metrics;

namespace Vertr.Experimental.MetricsApp;

public class ProductMetrics
{
    private readonly Counter<long> _pricingDetailsViewed;

    // myapp_products_pricing_page_requests_total
    public ProductMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("MyApp.Products");

        _pricingDetailsViewed = meter.CreateCounter<long>(
            "myapp.products.pricing_page_requests",
            unit: "requests",
            description: "The number of requests to the pricing details page for the product with the given product_id"
            );
    }

    public void PricingPageViewed(int id)
    {
        _pricingDetailsViewed.Add(delta: 1, new KeyValuePair<string, object?>("product_id", id));
    }
}