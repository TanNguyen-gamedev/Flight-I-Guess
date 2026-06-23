
using System;
using System.Collections.Generic;
using System.Numerics;

public class RunStateModel
{
    public event Action<int> OnTotalScrapChanged;
    public event Action<int> OnTotalCoresChanged;

    private int _totalScrap;
    private int _totalCores;

    private float MagnetRadiusSquare;

    private List<ScrapModel> _activeScraps = new List<ScrapModel>();

    public void SetMagnetRadius(float radius)
    {
        MagnetRadiusSquare = radius * radius;
    }

    public void AddScrapToTracking(ScrapModel scrap)
    {
        if (!_activeScraps.Contains(scrap))
        {
            _activeScraps.Add(scrap);
        }
    }

    public void Tick(Vector2 playerPosition)
    {
        foreach(var scrap in _activeScraps)
        {
            if(scrap.IsMagnetized)
            {
                continue;
            }

            float distanceSquare = Vector2.DistanceSquared(playerPosition, scrap.Position);
            if(distanceSquare < MagnetRadiusSquare)
            {
                scrap.Magnetize();
            }
        }
    }

    // Called by the Presenter when tween finsish the animation
    public void CollectScrap(ScrapModel scrapModel)
    {
        if (scrapModel.Type == ResourceType.Scrap)
        {
            _totalScrap += scrapModel.Amount;
            OnTotalScrapChanged?.Invoke(_totalScrap);
        }
        else if (scrapModel.Type == ResourceType.Core)
        {
            _totalCores += scrapModel.Amount;
            OnTotalCoresChanged?.Invoke(_totalCores);
        }
        
        _activeScraps.Remove(scrapModel);
    }

}