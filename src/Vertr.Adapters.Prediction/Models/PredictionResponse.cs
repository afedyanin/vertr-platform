namespace Vertr.Adapters.Prediction.Models;
internal record class PredictionResponse
{
    public DateTime[] Time { get; set; } = [];

    public Action[] Action { get; set; } = [];

    /*
    class PredictionResponse(BaseModel):
        time: List[datetime]
        action: List[Action]
     */
}
