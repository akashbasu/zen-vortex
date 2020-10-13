using UnityEngine;

namespace ZenVortex
{
    internal interface ITimedPowerupData : IBasePowerupData
    {
        float Duration { get; }
        float EffectData { get; }
        float ResetData { get; }
    }
    
    public class TimedPowerup : BasePowerup, ITimedPowerupData
    {
        [Header("Timed Data")]
        [SerializeField] private float _duration = GameConstants.Powerup.PowerupDuration;
        [SerializeField] private float _effectData;
        [SerializeField] private float _resetData  = 1;

        float ITimedPowerupData.Duration => _duration;
        float ITimedPowerupData.EffectData => _effectData;
        float ITimedPowerupData.ResetData => _resetData;
    }
}