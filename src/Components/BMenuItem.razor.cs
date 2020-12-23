﻿
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class BMenuItem : BComponentBase, IMenuItem
    {
        [Parameter]
        public string Index { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool Disabled { get; set; } = false;

        [Parameter]
        public string Route { get; set; }

        [Parameter]
        public object Model { get; set; }

        [Parameter]
        public string Icon { get; set; } = "el-icon-menu";

        [CascadingParameter]
        public BMenu TopMenu { get; set; }
        [CascadingParameter]
        public BMenuContainer Menu { get; set; }

        [CascadingParameter]
        public BSubMenu ParentMenu { get; set; }
        [CascadingParameter]
        public MenuOptions Options { get; set; }

        public virtual string TextColor { get; set; }
        public virtual string BorderColor { get; set; }

        public virtual bool IsActive { get; set; }
        public virtual string BackgroundColor { get; set; }

        private string currentRoute;
        public void Activate()
        {
            if (string.IsNullOrWhiteSpace(Route))
            {
                return;
            }
            IsActive = true;
            TextColor = Options.ActiveTextColor;
            BorderColor = Options.ActiveTextColor;
            BackgroundColor = Options.HoverColor;

        }
        public void DeActivate()
        {
            if (string.IsNullOrWhiteSpace(Route))
            {
                return;
            }
            IsActive = false;
            TextColor = Options.TextColor;
            BorderColor = "transparent";
            BackgroundColor = Options.BackgroundColor;
        }

        [Parameter]
        public EventCallback<BChangeEventArgs<string>> OnRouteChanging { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Func<string, bool> matchFunc = TopMenu.Match;
            if (matchFunc == null)
            {
                matchFunc = DefaultMenuMatcher;
            }
            if ((!string.IsNullOrWhiteSpace(Options.DefaultActiveIndex) && Options.DefaultActiveIndex == Index) || matchFunc(Route))
            {
                TopMenu.ActivateItem(this);
            }
        }

        private bool DefaultMenuMatcher(string route)
        {
            if (string.IsNullOrWhiteSpace(Route))
            {
                return false;
            }
            var localPath = NavigationManager.Uri.Replace(NavigationManager.BaseUri.TrimEnd('/'),"");
            //var uri = new Uri(NavigationManager.Uri);
            var paths = localPath.Split('/').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            var menuPaths = route.Split('/').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            if (paths.Length != menuPaths.Length)
            {
                return false;
            }
            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i].ToUpper() != menuPaths[i].ToUpper())
                {
                    return false;
                }
            }
            return true;
        }

        public void OnOver()
        {
            if (Options.Mode == MenuMode.Horizontal && ParentMenu != null)
            {
                ParentMenu.KeepSubMenuOpen();
            }
            if (string.IsNullOrWhiteSpace(Options.HoverColor))
            {
                return;
            }
            BackgroundColor = Options.HoverColor;
            RequireRender = true;
        }

        public void OnOut()
        {
            if (Options.Mode == MenuMode.Horizontal)
            {
                BackgroundColor = Options.BackgroundColor;
                return;
            }
            BackgroundColor = IsActive ? Options.HoverColor : Options.BackgroundColor;
            RequireRender = true;
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            Func<string, bool> matchFunc = TopMenu.Match;
            if (matchFunc == null)
            {
                matchFunc = DefaultMenuMatcher;
            }
            if (matchFunc(Route))
            {
                TopMenu.ActivateItem(this);
            }
            if (!string.IsNullOrWhiteSpace(BackgroundColor))
            {
                return;
            }
            BackgroundColor = Options.BackgroundColor;
            TextColor = Options.TextColor;
        }

        public async Task OnClickAsync()
        {
            if (ParentMenu != null && TopMenu.Mode == MenuMode.Horizontal)
            {
                await ParentMenu.CloseAsync();
            }
            TopMenu.ActivateItem(this);
            if (!string.IsNullOrEmpty(Route))
            {
                if (OnRouteChanging.HasDelegate)
                {
                    var arg = new BChangeEventArgs<string>();
                    arg.NewValue = Route;
                    arg.OldValue = currentRoute;
                    await OnRouteChanging.InvokeAsync(arg);
                    if (arg.DisallowChange)
                    {
                        return;
                    }
                }
                NavigationManager.NavigateTo(Route);
                NavigationManager.LocationChanged += NavigationManager_LocationChanged;
            }
        }

        private void NavigationManager_LocationChanged(object sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
        {
            NavigationManager.LocationChanged -= NavigationManager_LocationChanged;
            ParentMenu?.MarkAsRequireRender();
            TopMenu.MarkAsRequireRender();
            TopMenu.Refresh();
        }

        protected override bool ShouldRender()
        {
            return true;
        }
    }
}
