using System;
using System.Collections;
using UnityEngine;

namespace ZenVortex
{
    internal class SocialManager : MonoBehaviour, IInitializable
    {
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            GameEventManager.Subscribe(GameEvents.Application.Share, OnShare);
            GameEventManager.Subscribe(GameEvents.Application.Contact, OnContact);
            
            onComplete?.Invoke(this);
        }

        private void OnDestroy()
        {
            GameEventManager.Unsubscribe(GameEvents.Application.Share, OnShare);
            GameEventManager.Unsubscribe(GameEvents.Application.Contact, OnContact);
        }

        private void OnContact(object[] obj)
        {
            Application.OpenURL($"mailto:{GameConstants.Social.Contact.EmailId}?subject={GameConstants.Social.Contact.Subject}&body={GameConstants.Social.Contact.Content}");
        }

        private void OnShare(object[] obj)
        {
            StartCoroutine(Share());
        }

        private IEnumerator Share()
        {
            yield return new WaitForEndOfFrame();
            
            var ns = new NativeShare().SetSubject(GameConstants.Social.Share.Subject)
                .SetTitle(GameConstants.Social.Share.Content).SetText(string.Format(GameConstants.Social.Share.BodyFormat, PlayerDataProvider.LastRunScore))
                .AddFile(ScreenCapture.CaptureScreenshotAsTexture(), "HighScore.png");
            ns.Share();
        }
    }
    
    public static partial class GameConstants
    {
        internal static class Social
        {
            internal static class Contact
            {
                public const string EmailId = "akashbasu1992@gmail.com";
                public const string Subject = "Zen Vortex";
                public const string Content = "Hi!\nI played your game Zen Vortex and wanted to get in touch with you!\n";
            }
        
            internal static class Share
            {
                public const string Subject = "Zen Vortex";
                public const string Content = "Check out Zen Vortex!";
                public const string BodyFormat = "I scored {0}! Can you beat me?";
            }
        }
    }
}