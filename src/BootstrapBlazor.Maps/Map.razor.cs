// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace BootstrapBlazor.Components;
/// <summary>
/// 谷歌地图 Maps 组件
/// </summary>
public partial class Map : IAsyncDisposable
{
    [Inject] IJSRuntime? JS { get; set; }
    [Inject] IConfiguration? config { get; set; }

    /// <summary>
    /// 获得/设置 错误回调方法
    /// </summary>
    [Parameter]
    public Func<string, Task>? OnError { get; set; }

    /// <summary>
    /// 获得/设置 GoogleKey<para></para>
    /// 为空则在 IConfiguration 服务获取 "GoogleKey" , 默认在 appsettings.json 文件配置
    /// </summary>
    [Parameter]
    public string? GoogleKey { get; set; }

    /// <summary>
    /// 获得/设置 style
    /// </summary>
    [Parameter]
    public string Style { get; set; } = "height:700px;width:100%;";

    ElementReference map { get; set; }

    private IJSObjectReference? module;
    private string key = String.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            key = GoogleKey ?? (config != null ? config["GoogleKey"] : null) ?? "abcd";
            module = await JS.InvokeAsync<IJSObjectReference>("import", "./_content/Densen.Component.Blazor/lib/google/map.js");
            while (!(await Init()))
            {
                await Task.Delay(500);
            }

            //await module.InvokeVoidAsync("initMaps"  );
        }
    }


    public async Task<bool> Init() => await module!.InvokeAsync<bool>("addScript", new object?[] { key, map, null, null, null });

    public async Task OnBtnClick() => await module!.InvokeVoidAsync("addScript", new object?[] { key, map, null, null, null });

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (module is not null)
        {
            //await module.InvokeVoidAsync("destroy", Options);
            await module.DisposeAsync();
        }
    }
}
