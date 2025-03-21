using System.Collections.Generic;

// ReSharper disable once ClassNeverInstantiated.Global
public class SaveableContainer{
    readonly List<ISaveableEntity> m_SaveableEntities = new();
    public void Bind(ISaveableEntity saveableEntity) => m_SaveableEntities.Add(saveableEntity);
    public IEnumerable<ISaveableEntity> GetSaveableEntities() => m_SaveableEntities;
}