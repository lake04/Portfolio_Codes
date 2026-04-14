using UnityEngine;

public class MainUIInstaller : MonoBehaviour
{
    [SerializeField] private MainView mainView;

    private MainPresenter _presenter;

    private void Awake()
    {
        var model = new MainModel();
        model.Init(); 

        _presenter = new MainPresenter(mainView, model);

        model.UpdateTargetByIndex(2);
    }

    private void OnDestroy()
    {
        _presenter?.OnDestroy();
    }
}