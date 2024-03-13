using Common;
using UnityEngine;

namespace ResourceManagement
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        [field: SerializeField] public AssetResourcesManager AssetsResourcesManager { get; private set; }
        [field: SerializeField] public UIResourceManager UIResourcesManager { get; private set; }
    }
}