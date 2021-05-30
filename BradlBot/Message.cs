using System;
using System.Collections.Generic;
using DSharpPlus;
using DSharpPlus.Entities;

namespace BradlBot
{
    public class Message
    {
        public Guid UniqueVersionOfMessage = Guid.NewGuid();
        
        public DiscordChannel Channel { get; set; }
        public DiscordUser Author { get; set; }
        public string Content { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public DateTimeOffset EditedTimestamp { get; set; }
        public bool IsEdited { get; set; }
        public bool IsTTS { get; set; }
        public bool MentionEveryone { get; set; }
        public IReadOnlyList<DiscordUser> MentionedUsers { get; set; }
        public IReadOnlyList<DiscordRole> MentionedRoles { get; set; }
        public IReadOnlyList<DiscordChannel> MentionedChannels { get; set; }
        public IReadOnlyList<DiscordEmbed> Embeds { get; set; }
        public IReadOnlyList<DiscordReaction> Reactions { get; set; }
        public bool Pinned { get; set; }
        public ulong? WebhookID { get; set; }
        public MessageType? MessageType { get; set; }
        public ulong ID { get; set; }

        public static Message GetFromDiscordMessage(DiscordMessage dmessage)
        {
            return new Message()
            {
                Channel = dmessage.Channel,
                Author = dmessage.Author,
                Content = dmessage.Content,
                Timestamp = dmessage.Timestamp,
                EditedTimestamp = dmessage.EditedTimestamp ?? new DateTimeOffset(),
                IsEdited = dmessage.IsEdited,
                IsTTS = dmessage.IsTTS,
                MentionEveryone = dmessage.MentionEveryone,
                MentionedUsers = dmessage.MentionedUsers,
                MentionedRoles = dmessage.MentionedRoles,
                MentionedChannels = dmessage.MentionedChannels,
                Embeds = dmessage.Embeds,
                Reactions = dmessage.Reactions,
                Pinned = dmessage.Pinned,
                WebhookID = dmessage.WebhookId,
                MessageType = dmessage.MessageType,
                ID = dmessage.Id
            };
        }

        public static bool operator ==(Message e1, DiscordMessage e2)
        {
            if ((long?) e1.ID == (long?) e2.Id)
                return true;
            /*else*/ return false;
        }

        public static bool operator !=(Message e1, DiscordMessage e2)
        {
            return !(e1 == e2);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    
        // override object.Equals
        public override bool Equals(object obj)
        {
            return this == (Message) obj;
        }
    }
}