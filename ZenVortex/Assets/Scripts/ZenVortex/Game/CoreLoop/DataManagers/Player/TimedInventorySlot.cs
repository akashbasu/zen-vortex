using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZenVortex
{
    internal class TimedInventorySlot
    {
        private readonly PowerupType _powerupType;
        private readonly Queue<LTDescr> _instances;
        
        public event Action<PowerupType, float> OnEffectBegin;
        public event Action<PowerupType, float> OnEffectEnd;
        
        public TimedInventorySlot(PowerupType powerupData)
        {
            _powerupType = powerupData;
            _instances = new Queue<LTDescr>();
        }

        public void AddEffect(float powerupDataDuration, float effectData, float resetData)
        {
            Debug.Log($"[{nameof(TimedInventorySlot)}] {nameof(AddEffect)} Effect {_powerupType} expires at {DateTime.Now.AddSeconds(powerupDataDuration)}");
            
            var delayedTween = LeanTween.delayedCall(powerupDataDuration, () => OnEffectExpire(resetData));
            _instances.Enqueue(delayedTween);
            
            OnEffectBegin?.Invoke(_powerupType, effectData);
        }
        
        public void Reset()
        {
            foreach (var instance in _instances) StopTween(instance);
        }

        private static void StopTween(LTDescr instance)
        {
            if(instance == null) return;
            
            LeanTween.cancel(instance.uniqueId);
            instance.reset();
        }

        private void OnEffectExpire(float resetData)
        {
            _instances.Dequeue();
            
            Debug.Log($"[{nameof(TimedInventorySlot)}] {nameof(OnEffectExpire)} Effect {_powerupType} in expired");

            OnEffectEnd?.Invoke(_powerupType, resetData);
        }
    }
}