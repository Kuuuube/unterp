using System.Numerics;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Tablet;

namespace SpringInterpolator
{
    [PluginName("SpringInterpolator")]
    public class SpringInterpolator : AsyncPositionedPipelineElement<IDeviceReport>
    {
        // future idea: make the z springy (pressure)
        private readonly SpringSettings2D settings;
        private readonly Spring2D spring;
        private uint pressure;

        public SpringInterpolator()
        {
            // improvement/tweak: make the default target the current position of the cursor
            settings = new SpringSettings2D(0.4f, 0.3f, 100f);
            spring = new Spring2D(settings);
        }
        
        [SliderProperty("Stiffness", 0.0001f, 100f, 1.5f), DefaultPropertyValue(1.5f),
        ToolTip("Controls how stiff the feeling is for the cursor to catch up to the pen\n" +
                "Lower -> Stiffer (Default 1.5)")]
        public float Stiffness
        {
            get => settings.Stiffness;
            set => settings.Stiffness = value;
        }
        
        [SliderProperty("Damping", 0.0001f, 100f, 3f), DefaultPropertyValue(3f),
        ToolTip("Damping controls how much \"friction\" there is\n" +
                "Higher -> More friction (Default: 3.0)")]
        public float Damping
        {
            get => settings.Damping;
            set => settings.Damping = value;
        }
        
        [SliderProperty("StepSize", 1f, 5000f, 40f), DefaultPropertyValue(40f), ToolTip(
             "Controls how fast the spring simulation is run\n" +
             "Higher -> Slower (Default: 40.0)")
        ]
        public float StepSize
        {
            get => settings.StepSize;
            set => settings.StepSize = value;
        }

        protected override void UpdateState()
        {
            if (State is ITabletReport report)
            {
                report.Position = spring.Update();
                report.Pressure = pressure;
                State = report;
            }

            if (PenIsInRange())
            {
                OnEmit();
            }
        }

        protected override void ConsumeState()
        {
            if (State is not ITabletReport report) return;
            spring.Target = new Vector2(report.Position.X, report.Position.Y);
            spring.Update();
            pressure = report.Pressure;
        }
        public override PipelinePosition Position => PipelinePosition.PreTransform;
    }
}