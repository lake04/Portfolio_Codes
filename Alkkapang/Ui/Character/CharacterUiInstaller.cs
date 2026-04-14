using UnityEngine;

public class CharacterUiInstaller : Singleton<CharacterUiInstaller>
{
    [SerializeField] private CharacterView characterView;

    public CharacterPresenter _presenter;

    public override void Awake()
    {
        var model = new CharacterModel();

        _presenter = new CharacterPresenter(characterView, model);
    }
}
