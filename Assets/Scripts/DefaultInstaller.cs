using UnityEngine.InputSystem.UI;
using Zenject;

public class DefaultInstaller : MonoInstaller{
    public override void InstallBindings(){
        Container.Bind<FactoryRunner>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<InputSystemUIInputModule>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<UIActionHandler>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<SaveableContainer>().AsSingle();
        Container.Bind<Inventory>().AsSingle();
    }
}