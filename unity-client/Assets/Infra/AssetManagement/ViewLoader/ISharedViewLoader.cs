using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Infra.AssetManagement.ViewLoader
{
    public interface ISharedViewLoader<TViewInterface>
    {
        TViewInterface? CachedView { get; }
        UniTask<TViewInterface> Load(ICollection<IDisposable> resources, CancellationToken cancellationToken, Transform parent);
    }
	
    public interface ISharedViewLoader<TViewInterface, in TKey>
    {
        TViewInterface? CachedView { get; }
        UniTask<TViewInterface> Load(TKey key, ICollection<IDisposable> resources, CancellationToken cancellationToken, Transform parent);
    }
}