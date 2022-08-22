namespace Mara.Lib.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test, Order(0)]
    public void WriteUwu()
    {
        var m_owos = new List<OWO>();
        for (var i = 0; i < (int)MaraPlatform.None; i++)
        {
            var o = new OWO(Resource_test.test, (MaraPlatform)i);
            m_owos.Add(o);
        }

        Utils.WriteUWU(Path.Combine(Path.GetTempPath(), "output.UWU"), m_owos.ToArray());
        
        if (File.Exists(Path.Combine(Path.GetTempPath(), "output.UWU")))
            Assert.Pass();
        else
            Assert.Fail();
    }

    [Test, Order(1)]
    public void ReadUwu()
    {
        var uwu = Utils.ReadUWU(Path.Combine(Path.GetTempPath(), "output.UWU"));

        if (uwu.m_entry.Length <= 0)
            Assert.Fail();
        else
            Assert.Pass();
    }

    [Test]
    public void GetPlatforms()
    {
        var platforms = Utils.GetPlatformsFromUWU(Path.Combine(Path.GetTempPath(), "output.UWU"));

        if (platforms != null)
            Assert.Pass();
        else
            Assert.Fail();
    }

    [Test]
    public void SearchPlatform()
    {
        var platforms = Utils.SearchOWO(Path.Combine(Path.GetTempPath(), "output.UWU"), MaraPlatform.NintendoSwitch);
        
        if (platforms.m_platform == MaraPlatform.NintendoSwitch)
            Assert.Pass();
        else
            Assert.Fail();
    }
    
    [Test, Order(2)]
    public void LoadOWO()
    {
        var platforms = Utils.SearchOWO(Path.Combine(Path.GetTempPath(), "output.UWU"), MaraPlatform.Generic);
        var mara = new Mara.Lib.Platforms.Generic.Main("", "", platforms);
    }
}