using Unity.VisualScripting;
using UnityEngine;

public class CharacterDetailPresenter : PresenterBase<CharacterDetailView, CharacterDetailModel>
{
    public CharacterDetailPresenter(CharacterDetailView view, CharacterDetailModel model) : base(view, model)
    {
        OnInitialize();
    }

    public override void OnDestroy()
    {
        View.OnClickClose -= View.Close;
        View.OnClickSelect -= Model.SetSelectState;
        View.OnClickSelect -= View.Close;
    }

    public override void OnInitialize()
    {
        View.OnClickClose += View.Close;
        View.OnClickSelect += Model.SetSelectState;
        View.OnClickSelect += View.Close;
    }

    public void SetData(CharacterMasterData data, CharacterUiData uiData)
    {
        Model.characterData = data;

        View.SetCharacterInfo(data, uiData);
    }


}
