using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SmallGame.Services
{

    public interface IResourceService : CoreGameService
    {
        void Configure(ContentManager content);
        C Load<C>(string filePath);
    }

    public class ResourceService : IResourceService
    {

        public ContentManager Content { get; set; }
        private Dictionary<string, object> _resourceMap;

        public ResourceService()
        {
            _resourceMap = new Dictionary<string, object>();
        }

        public void Configure(ContentManager content)
        {
            Content = content;
        }

        public C Load<C>(string filePath)
        {
            if (!_resourceMap.ContainsKey(filePath))
            {
                _resourceMap.Add(filePath, Content.Load<C>(filePath));
            }
            return (C) _resourceMap[filePath];
        }

    }
}
