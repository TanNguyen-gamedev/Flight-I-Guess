
using System;
using System.Numerics;

public enum ResourceType
{
    Scrap,
    Core
}

public class ScrapModel
{
    private int _amount;
    public int Amount => _amount;
    
    private ResourceType _type;
    public ResourceType Type => _type;
    
    private bool _isMagnetized;

    public bool IsMagnetized => _isMagnetized;
    public event Action OnMagnetized;
    public Vector2 Position;

    public void Init(int amount, ResourceType type, Vector2 startPosition)
    {
        _amount = amount;
        _type = type;
        _isMagnetized = false;
        Position = startPosition;
    }

    public void Magnetize()
    {
        if(_isMagnetized)
        {
            return;
        }

        _isMagnetized = true;
        OnMagnetized?.Invoke();
    }
}