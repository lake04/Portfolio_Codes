using System.Collections.Generic;
using Unity;
using UnityEngine;

public class CharacterPresenter : PresenterBase<CharacterView, CharacterModel>
{
    public CharacterPresenter(CharacterView view, CharacterModel model) : base(view, model)
    {
        OnInitialize();
    }

    public override void OnInitialize()
    {
        View.onClickSortButton +=  OnClickSortButton;
        View.onClickAscendingButton += OnClickAscendingButton;

    }

    public override void OnDestroy()
    {
        View.onClickSortButton -= OnClickSortButton;
        View.onClickAscendingButton -= OnClickAscendingButton;
    }

    private void OnClickSortButton()
    {
        Model.SortCharacterList();
        View.Render(Model.CharacterList);
        UpdateSortButtonText();
    }

    private void OnClickAscendingButton()
    {
        Model.Ascending();
        UpdateAscendingButtonSprite();
        View.Render(Model.CharacterList);
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

    private void UpdateSortButtonText()
    {
        switch (Model.CurrentSortType)
        {
            case CharacterSortType.Name:
                View.SetSortButtonText("이름순");
                break;
            case CharacterSortType.Rarity:
                View.SetSortButtonText("등급순");
                break;
            //case CharacterSortType.Level:
            //    View.SetSortButtonText("레벨순");
            //    break;
            //case CharacterSortType.Acquired:
            //    View.SetSortButtonText("획득순");
            //    break;
        }
    }

    private void UpdateAscendingButtonSprite()
    {
        View.SetAscendingButtonSprite(Model.SortAscending);
    }
}