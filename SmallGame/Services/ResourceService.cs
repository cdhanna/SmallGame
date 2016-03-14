using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace SmallGame.Services
{
    /// <summary>
    /// The ResourceService allows assets to be loaded
    /// </summary>
    public interface IResourceService : IGameService
    {
        /// <summary>
        /// Configures the service 
        /// </summary>
        /// <param name="content">The ContentManager used to load assets</param>
        void Configure(ContentManager content);

        /// <summary>
        /// Load an asset
        /// </summary>
        /// <typeparam name="C">The type of asset to load</typeparam>
        /// <param name="filePath">the file path of the asset</param>
        /// <returns>an Instance of C, the asset that was loaded.</returns>
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
