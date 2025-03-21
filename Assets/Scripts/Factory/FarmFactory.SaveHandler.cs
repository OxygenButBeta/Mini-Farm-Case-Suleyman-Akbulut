using System;


/// <summary>
/// Handles saving and loading of factory data. When loading, adjusts the factory's state based on the
/// elapsed time since the last save, updating the order count, stock, and time until the next production.
/// Also handles saving the current factory state, including stock, order count, and production time.
/// </summary>
public partial class FarmFactory{
    public override RecordContainer ExtractSaveData(){
        return new RecordContainer()
            .AddRecord(OrderCount.Value, nameof(OrderCount))
            .AddRecord(CurrentStock.Value, nameof(CurrentStock))
            .AddRecord(SecondsUntilNextProduction.Value, nameof(SecondsUntilNextProduction))
            .AddRecord(DateTime.UtcNow, "Time");
    }

    /// <summary>
    /// This method is responsible for loading the saved data into the factory.
    /// </summary>
    /// <param name="data"></param>
    public override void LoadData(RecordContainer data){
        OrderCount.Value = data.GetRecord<int>(nameof(OrderCount));
        CurrentStock.Value = data.GetRecord<int>(nameof(CurrentStock));
        var saved_seconds = data.GetRecord<int>(nameof(SecondsUntilNextProduction));
        DateTime record_time = data.GetRecord<DateTime>("Time");
        
        var elapsedTime = (DateTime.UtcNow - record_time).TotalSeconds;

        if (FactorySO.NoResourceNecessary){
            if (saved_seconds > elapsedTime){
                SecondsUntilNextProduction.Value = saved_seconds - (int)elapsedTime;
                return;
            }

            var producedCount = (int)(elapsedTime / FactorySO.ProductionDuration);
            var excessTime = (int)elapsedTime % FactorySO.ProductionDuration;

            CurrentStock.Value += producedCount;
            SecondsUntilNextProduction.Value = FactorySO.ProductionDuration - excessTime;
            if (CurrentStock.Value > FactorySO.StockCapacity){
                CurrentStock.Value = FactorySO.StockCapacity;
            }

            return;
        }

        if (OrderCount.Value == 0){
            SecondsUntilNextProduction.Value = 0;
            return;
        }

        if (saved_seconds > elapsedTime){
            SecondsUntilNextProduction.Value -= (int)elapsedTime;
            return;
        }


        if (elapsedTime > OrderCount.Value * FactorySO.ProductionDuration){
            CurrentStock.Value += OrderCount.Value;
            OrderCount.Value = 0;
            SecondsUntilNextProduction.Value = 0;
            return;
        }

        elapsedTime -= SecondsUntilNextProduction.Value;
        CurrentStock.Value++;
        OrderCount.Value--;

        var producibleCount = (int)(elapsedTime / FactorySO.ProductionDuration);
        CurrentStock.Value += producibleCount;
        OrderCount.Value -= producibleCount;

        if (OrderCount.Value > 0)
            SecondsUntilNextProduction.Value = FactorySO.ProductionDuration - (int)elapsedTime -
                                               (producibleCount * FactorySO.ProductionDuration);
    }
}