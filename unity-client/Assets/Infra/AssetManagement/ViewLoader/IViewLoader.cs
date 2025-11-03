using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Infra.AssetManagement.ViewLoader
{
    public interface IViewLoader<TViewInterface>
    {
        UniTask<TViewInterface> Load(ICollection<IDisposable> resources, CancellationToken cancellationToken,
            Transform parent);
    }

    public interface IViewLoader<TViewInterface, in TKey>
    {
        UniTask<TViewInterface> Load(TKey key, ICollection<IDisposable> resources, CancellationToken cancellationToken,
            Transform parent);
    }
}