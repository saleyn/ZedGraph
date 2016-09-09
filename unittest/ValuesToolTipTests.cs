namespace ZedGraph
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Reflection;
    using System.Windows.Forms;

    using NUnit.Framework;

    using Ploeh.Albedo;
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.AutoRhinoMock;
    using Ploeh.AutoFixture.Idioms;

    using Rhino.Mocks;

    [TestFixture]
    internal class ValuesToolTipTests
    {
        [Test]
        public void GuardClauses()
        {
            IFixture fixture = new Fixture().Customize(new AutoRhinoMockCustomization());
            fixture.Customize<Control>(c => c.OmitAutoProperties());
            fixture.Customizations.Add(new PointBuilder());
            var methods = new Methods<ValuesToolTip>();
            var members = new List<MemberInfo>(typeof(ValuesToolTip).GetMembers());
            members.Remove(methods.Select(tt => tt.Set(default(string))));
            members.Remove(methods.Select(tt => tt.Set(default(string), default(Point))));

            fixture.Create<GuardClauseAssertion>()
                .Verify(members);
        }

        [Test]
        public void Create_EnableCreatedToolTip_SetsToolTipActive()
        {
            var toolTip = new ToolTip { Active = false };
            ValuesToolTip sut = ValuesToolTip.Create(new Control(), toolTip);

            sut.Active = true;

            Assert.That(toolTip.Active, Is.True, "The non-active tool tip was not set to active.");
        }

        [Test]
        public void Create_DisableCreatedToolTip_SetsToolTipNotActive()
        {
            var toolTip = new ToolTip { Active = true };
            ValuesToolTip sut = ValuesToolTip.Create(new Control(), toolTip);

            sut.Active = false;

            Assert.That(toolTip.Active, Is.False, "The active tool tip was still active.");
        }
    }
}
