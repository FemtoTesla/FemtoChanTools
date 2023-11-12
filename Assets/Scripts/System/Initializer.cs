using Femto.MasterData;
using MasterMemory;
using MessagePack;
using MessagePack.Resolvers;
using UnityEngine;

public static class Initializer
{
    // MessagePack の初期化処理
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        StaticCompositeResolver.Instance.Register
        (
            MasterMemoryResolver.Instance,
            GeneratedResolver.Instance,
            StandardResolver.Instance
        );

        var options = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);
        MessagePackSerializer.DefaultOptions = options;
    }
}