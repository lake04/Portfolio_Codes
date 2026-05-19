using BackEnd;
using Cysharp.Threading.Tasks;
using System;
using System.Diagnostics;

public class MainPresenter : PresenterBase<MainView, MainModel>
{
    public MainPresenter(MainView view, MainModel model) : base(view, model)
    {
        OnInitialize();
    }

    public override void OnInitialize()
    {
        View.OnTabBtnClicked += (index) => Model.UpdateTargetByIndex(index);
        View.OnEndDragEvent += (val, delta) => Model.UpdateTargetByScrollValue(val, delta.x);

        Model.OnTabStateChanged += (index, pos) =>
        {
            View.RenderTabState(index, pos);
        };

        LoadAndApplyNicknameAsync().Forget();
    }

    public override void OnDestroy()
    {
        View.OnEndDragEvent = null;
        View.OnTabBtnClicked = null;
        Model.OnTabStateChanged = null;
    }

    private async UniTaskVoid LoadAndApplyNicknameAsync()
    {
        var ucs = new UniTaskCompletionSource<BackendReturnObject>();

        SendQueue.Enqueue(Backend.BMember.GetUserInfo, (callback) =>
        {
            ucs.TrySetResult(callback);
        });

        BackendReturnObject bro = await ucs.Task;

        if (bro.IsSuccess())
        {
            LitJson.JsonData row = bro.GetReturnValuetoJSON()["row"];

            if (row.Keys.Contains("nickname") && row["nickname"] != null)
            {
                string nickname = row["nickname"].ToString();

                View.SetNickname(nickname);
            }
            else
            {
                View.SetNickname("GUEST");
            }
        }
        else
        {
            View.SetNickname("GUEST");
        }
    }

}