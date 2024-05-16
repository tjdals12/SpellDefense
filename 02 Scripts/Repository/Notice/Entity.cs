using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.Notice {
    public enum NoticeType {
        Announcement = 0,
        Update
    }

    public class Entity
    {
        public int id { get; private set; }
        public NoticeType type { get; private set; }
        public Dictionary<string, string> titles { get; private set; }
        public Dictionary<string, string> contents { get; private set; }
        public DateTime createdAt { get; private set; }
        public Entity(int id, NoticeType type, Dictionary<string, string> titles, Dictionary<string, string> contents, DateTime createdAt) {
            this.id = id;
            this.type = type;
            this.titles = titles;
            this.contents = contents;
            this.createdAt = createdAt;
        }

        public string GetTitle(string language) {
            if (this.titles.ContainsKey(language)) {
                return this.titles[language];
            }
            return this.titles["en"];
        }

        public string GetContent(string language) {
            if (this.contents.ContainsKey(language)) {
                return this.contents[language];
            }
            return this.contents["en"];
        }
    }
}
