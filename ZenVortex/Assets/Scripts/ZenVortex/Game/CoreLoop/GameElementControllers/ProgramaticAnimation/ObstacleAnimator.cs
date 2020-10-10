using System;
using UnityEngine;

namespace ZenVortex
{
    internal class ObstacleAnimator
    {
        internal readonly struct AnimateInParams
        {
            //Movement
            public readonly float distanceToTravel;
            public readonly float timeToTravel;
            
            //Animate in
            public readonly float animateInTime;
            public readonly Vector3 targetScale;
            
            //Rotation
            public readonly float targetRotation;
            public readonly float rotationTime;
            
            //Color
            public readonly Color groupColor;

            public AnimateInParams(float distanceToTravel, float timeToTravel, float animateInTime, Vector3 targetScale, float targetRotation, float rotationTime, Color groupColor)
            {
                this.distanceToTravel = distanceToTravel;
                this.timeToTravel = timeToTravel;
                this.animateInTime = animateInTime;
                this.targetScale = targetScale;
                this.targetRotation = targetRotation;
                this.rotationTime = rotationTime;
                this.groupColor = groupColor;
            }
        }
        
        internal readonly struct AnimateOutParams
        {
            //Animate in
            public readonly float animateOutTime;
            public readonly Vector3 targetScale;

            public AnimateOutParams(Vector3 targetScale, float animateOutTime)
            {
                this.targetScale = targetScale;
                this.animateOutTime = animateOutTime;
            }
        }
        
        private readonly GameObject _go;
        
        internal ObstacleAnimator(GameObject go)
        {
            _go = go;
        }
        
        public void AnimateIn(AnimateInParams animateInParams, Action onComplete)
        {
            LeanTween.sequence()
                .append(() => StartMovement(animateInParams.distanceToTravel, animateInParams.timeToTravel, onComplete))
                .append(() => AnimateScale(animateInParams.targetScale, animateInParams.animateInTime))
                .append(() => AnimateColor(animateInParams.groupColor, 1, animateInParams.animateInTime))
                .append(animateInParams.animateInTime)
                .append(() => StartRotation(animateInParams.targetRotation, animateInParams.rotationTime));
        }

        public void Pause()
        {
            LeanTween.pause(_go);
        }

        public void AnimateOut(AnimateOutParams animateOutParams)
        {
            AnimateScale(animateOutParams.targetScale, animateOutParams.animateOutTime);
        }

        public void Reset()
        {
            LeanTween.cancel(_go);
            AnimateColor(Color.clear, 0, 0);
        }
        
        private void StartMovement(float distanceToTravel, float time, Action onComplete)
        {
            _go.LeanMoveLocalZ(-distanceToTravel, time).setOnComplete(onComplete);
        }
        
        private void AnimateScale(Vector3 targetScale, float time)
        {
            _go.LeanScale(targetScale, time).setEase(GameConstants.Animation.Obstacle.Ease);
        }
        
        private void AnimateColor(Color targetColor, float alpha, float time)
        {
            _go.LeanColor(targetColor, 0);
            _go.LeanAlpha(alpha, time);
        }
        
        private void StartRotation(float targetRotation, float time)
        {
            _go.LeanRotateZ(targetRotation, time).setEase(GameConstants.Animation.Obstacle.Ease); 
        }
    }
}