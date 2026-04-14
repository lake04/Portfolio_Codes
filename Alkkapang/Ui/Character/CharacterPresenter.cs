using System.Collections.Generic;
using Unity;
using UnityEngine;

public class CharacterPresenter : PresenterBase<CharacterView, CharacterModel>
{
    public CharacterPresenter(CharacterView view, CharacterModel model) : base(view, model)
    {
        
    }

    public override void OnInitialize()
    {
    }

    public override void OnDestroy()
    {
    }

    public void RefreshCharacterList()
    {
        var masters = CharacterDataManager.Instance.GetAllCharacters();
        var list = new List<CharacterItemViewData>();

        foreach (var master in masters)
        {
            list.Add(new CharacterItemViewData
            {
                masterData = master,
                isOwned = CharacterDataManager.Instance.IsOwned(master.characterId),
                isEquipped = CharacterDataManager.Instance.IsEquipped(master.characterId)
            });
        }

        Model.SetCharacterList(list);
        View.Render(Model.CharacterList);
    }
}