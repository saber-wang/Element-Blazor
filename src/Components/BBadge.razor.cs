using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class BBadge : BComponentBase
    {
        internal HtmlPropertyBuilder cssClassBuilder;

        [Parameter]
        public int Value { get; set; }
        /// <summary>
        /// Badge 类型
        /// </summary>
        [Parameter]
        public BadgeType Type { get; set; } = BadgeType.Info;

        /// <summary>
        /// 小圆点
        /// </summary>
        [Parameter]
        public bool IsDot { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// 是否将自定义的 CSS 类加入到已有 CSS 类，如果为 false，则替换掉默认 CSS 类，默认为 true
        /// </summary>
        [Parameter]
        public bool AppendCustomCls { get; set; } = true;
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            cssClassBuilder = HtmlPropertyBuilder.CreateCssClassBuilder();
            if (string.IsNullOrWhiteSpace(Cls) || AppendCustomCls)
            {
                cssClassBuilder.Add($"el-badge",  Cls);
                return;
            }
            cssClassBuilder.AddIf(!string.IsNullOrWhiteSpace(Cls), Cls);
            if (string.IsNullOrWhiteSpace(Cls))
            {
                cssClassBuilder.Add($"el-badge");
            }
        }
    }
}
