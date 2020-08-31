using System;
using UnityEngine;

namespace RollyVortex
{
    internal class SocialManager : IInitializable
    {
        public void Initialize(Action<IInitializable> onComplete = null, params object[] args)
        {
            GameEventManager.Subscribe(GameEvents.Application.Share, OnShare);
            GameEventManager.Subscribe(GameEvents.Application.Contact, OnContact);
            
            onComplete?.Invoke(this);
        }

        private void OnContact(object[] obj)
        {
            Application.OpenURL($"mailto:{GameConstants.Contact.EmailId}?subject={GameConstants.Contact.EmailSubject}&body={GameConstants.Contact.EmailContent}");
        }

        private void OnShare(object[] obj)
        {
            throw new NotImplementedException();
        }
    }
    
    public static partial class GameConstants
    {
        internal static class Contact
        {
            public const string EmailId = "akashbasu1992@gmail.com";
            public const string EmailSubject = "Zen Vortex";
            public const string EmailContent = "Hi!\nI played your game Zen Vortex and wanted to get in touch with you!\n";
        }
    }
}