﻿namespace TeslaSolarCharger.Model.Entities.TeslaSolarCharger;

public class CachedCarState
{
    public int Id { get; set; }
    public int CarId { get; set; }
    //Needs to be nullable for new/unused cars
    public string? CarStateJson { get; set; }
    public DateTime LastUpdated { get; set; }
}