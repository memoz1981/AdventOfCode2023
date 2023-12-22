using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace AdventOfCode2023.API.Controllers;

[Route("day20")]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class _20_Day20_Controller : ControllerBase
{
    [HttpGet("exercise1")]
    public IActionResult Exercise1(int count)
    {
        var lines = System.IO.File.ReadAllLines("data20.txt").ToList();

        var manager = new ModuleContainer(lines);

        var total = manager.StartSequence(count);

        return Ok(total); 
    }

    [HttpGet("exercise2")]
    public IActionResult Exercise2()
    {
        var lines = System.IO.File.ReadAllLines("data20.txt").ToList();

        var manager = new ModuleContainer(lines);

        var result = manager.SendLowToRx();

        return Ok(result);
    }

    [HttpGet("exercise2test")]
    public IActionResult Exercise2Test(int count)
    {
        var lines = System.IO.File.ReadAllLines("data20.txt").ToList();

        var manager = new ModuleContainer(lines);

        var modules = manager.FindAllFlipFlopSourcesOfRx(); 

        var result = manager.StartSequenceReturnState(count);

        return Ok(result);
    }
}

public abstract class Module
{
    public Module(string name, PulseQueueManager pulseManager)
    {
        PulseManager = pulseManager;
        Name = name;
        DestinationModules = new();
        SourceModules = new(); 
    }

    public List<Module> SourceModules { get; set; }

    public void AddDestinations(List<Module> modules)
    {
        DestinationModules.AddRange(modules);

        foreach (var module in modules)
            module.SourceModules.Add(this); 
    }

    public virtual string ReturnState()
    {
        return $"{Name}, {ModuleType}, ";
    }

    protected PulseQueueManager PulseManager { get; set; }

    public string Name { get; set; }

    public ModuleType ModuleType { get; set; }

    public void Receive(Pulse pulse)
    {
        var pulseState = GeneratePulseState(pulse);

        if (pulseState == null)
            return; 
        
        foreach (var module in DestinationModules)
        {
            var pulseToSend = new Pulse(this, module, pulseState.Value); 
            PulseManager.ReceivePulse(pulseToSend);
        }
    }

    protected abstract bool? GeneratePulseState(Pulse pulse); 

    public List<Module> DestinationModules { get; set; }
}

public class BroadcasterModule : Module
{
    public BroadcasterModule(string name, PulseQueueManager pulseManager) : base(name, pulseManager)
    {
        ModuleType = ModuleType.BroadCaster; 
    }

    protected override bool? GeneratePulseState(Pulse pulse) => false; 
}

public class ConjunctionModule : Module
{
    public ConjunctionModule(string name, PulseQueueManager pulseManager) : base(name, pulseManager)
    {
        ModuleType = ModuleType.Conjunction;
    }

    public void AddSource(Module module)
    {
        _lastStates.Add(module.Name, false); 
    }

    protected override bool? GeneratePulseState(Pulse pulse)
    {
        _lastStates[pulse.From.Name] = pulse.State;

        if (_lastStates.Values.All(m => m == true))
            return false;

        return true; 
    }

    public override string ReturnState()
    {
        var baseState = base.ReturnState();

        return $"{baseState}, {string.Join(';', _lastStates.Select(m => $"{m.Key}-{m.Value.ToString()}"))}";
    }

    private Dictionary<string, bool> _lastStates = new(); 
}

public class DummyModule : Module
{
    public DummyModule(string name, PulseQueueManager pulseManager) : base(name, pulseManager)
    {
        ModuleType = ModuleType.Dummy;
    }

    protected override bool? GeneratePulseState(Pulse pulse) => null; 
}

public class FlipFlopModule : Module
{
    public FlipFlopModule(string name, PulseQueueManager pulseManager) : base(name, pulseManager)
    {
        ModuleType = ModuleType.FlipFlop;
    }

    private bool _state = false;

    public bool GetState() => _state; 

    protected override bool? GeneratePulseState(Pulse pulse)
    {
        if (pulse.State)
            return null;

        _state = !_state;

        return _state; 
    }

    public override string ReturnState()
    {
        if (_state == false)
            return "0";

        return "1"; 
    }
}

public class PulseQueueManager
{
    public PulseQueueManager(ModuleContainer container)
    {
        _container = container;
        _pulses = new();
        _highPulseCount = 0;
        _lowPulseCount = 0;
        _totalBySequence = new();
        _sequenceNumber = 0; 
    }

    private Dictionary<long, (long highCount, long lowCount)> _totalBySequence;

    private long _sequenceNumber; 

    public void AddModules(Dictionary<string, Module> modules)
    {
        _modules = modules; 
    }

    private ModuleContainer _container; 

    private Dictionary<string, Module> _modules; 

    public void Initialize()
    {
        _sequenceNumber++;
        _highPulseCount = 0;
        _lowPulseCount = 0; 
        _modules["broadcaster"].Receive(new Pulse(new ConjunctionModule("Button Module", this), _modules["broadcaster"], false));
        _lowPulseCount++; 

        while (true)
        {
            StartSequence();

            Task.Delay(100); 
            if (_pulses.Count == 0)
                break; 
        }

        _totalBySequence[_sequenceNumber] = (_highPulseCount, _lowPulseCount); 

        return; 
    }

    public bool SendLowToRx(out long buttonPressCount)
    {
        buttonPressCount = 0; 
        while (true)
        {
            _modules["broadcaster"].Receive(new Pulse(new ConjunctionModule("Button Module", this), _modules["broadcaster"], false));
            buttonPressCount++;

            while (true)
            {
                var result = StartSequenceLowToRx();

                if (result)
                    return true; 

                Task.Delay(100);
                if (_pulses.Count == 0)
                    break;
            }
        }
    }

    private bool StartSequenceLowToRx()
    {
        while (_pulses.Count > 0)
        {
            var firstPulse = _pulses.First();

            if (firstPulse.To.Name == "rx" && !firstPulse.State)
            {
                return true; 
            }

            var destination = _modules[firstPulse.To.Name];

            destination.Receive(firstPulse);

            _pulses.RemoveAt(0);

            Task.Delay(10);
        }
        return false; 
    }


    private void StartSequence()
    {
        while (_pulses.Count > 0)
        {
            var firstPulse = _pulses.First();



            var destination = _modules[firstPulse.To.Name];

            destination.Receive(firstPulse);

            _pulses.RemoveAt(0);

            Task.Delay(10);

            if (firstPulse.State)
                _highPulseCount++;
            else
                _lowPulseCount++;
        }
    }

    public long TotalPulses()
    {
        return _totalBySequence.Values.Sum(m => m.highCount) * _totalBySequence.Values.Sum(m => m.lowCount); 
    }

    private List<Pulse> _pulses { get; set; }

    public void ReceivePulse(Pulse pulse)
    {
        _pulses.Add(pulse);
    }

    private long _highPulseCount;

    private long _lowPulseCount; 
}

public class ModuleContainer
{
    public ModuleContainer(List<string> lines)
    {
        _pulseQueueManager = new PulseQueueManager(this);
        _modules = new(); 
        AddModules(lines, out var namesByIndex); 

        AddSourceAndDestinations(lines, namesByIndex, out var dummyModules);
        foreach (var dummy in dummyModules)
        {
            _modules[dummy.Name] = dummy; 
        }
        _pulseQueueManager.AddModules(_modules); 
    }

    private void AddModules(List<string> lines, out string[] namesByIndex)
    {
        namesByIndex = new string[lines.Count];
        for (int i=0; i<lines.Count;i++)
        {
            var line = lines[i]; 
            if (line[0] == '&')
            {
                var index = line.IndexOf(' ');
                var name = line.Substring(1, index - 1);

                var module = new ConjunctionModule(name, _pulseQueueManager);
                _modules.Add(name, module);
                namesByIndex[i] = name;
            }
            else if (line[0] == '%')
            {
                var index = line.IndexOf(' ');
                var name = line.Substring(1, index - 1);

                var module = new FlipFlopModule(name, _pulseQueueManager);
                _modules.Add(name, module);
                namesByIndex[i] = name;
            }
            else if (line.Contains("broadcaster"))
            {
                var module = new BroadcasterModule("broadcaster", _pulseQueueManager);
                _modules.Add("broadcaster", module);
                namesByIndex[i] = "broadcaster";
            }
            else
            {
                throw new ArgumentException(); 
            }
        }
    }

    private void AddSourceAndDestinations(List<string> lines, string[] namesByIndex, out List<Module> dummyModules)
    {
        dummyModules = new(); 
        
        for (int i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            var module = _modules[namesByIndex[i]];

            //add destinations
            var index = line.IndexOf('>');

            var destinations = line.
                Substring(index + 1, line.Length - index - 1)
                .Trim().Split(',').Select(m => m.Trim()).Where(m => _modules.ContainsKey(m))
                .Select(m => _modules[m]).ToList();

            var dummyDestinations = line.
                Substring(index + 1, line.Length - index - 1)
                .Trim().Split(',').Select(m => m.Trim()).Where(m => !_modules.ContainsKey(m)).ToList();

            if (dummyDestinations.Any())
            {
                List<Module> dummy = dummyDestinations.Select(m => (Module)new DummyModule(m, _pulseQueueManager)).ToList(); 
                
                dummyModules.AddRange(dummy);

                module.AddDestinations(dummy); 
            }

            module.AddDestinations(destinations);

            //add sources for conjunction modules
            var conjunctionDestinations = destinations.Where(m => m.ModuleType == ModuleType.Conjunction).ToList();

            foreach (var dest in conjunctionDestinations)
            {
                var conj = dest as ConjunctionModule;
                conj.AddSource(module); 
            }
        }
    }

    private Dictionary<string, Module> _modules { get; set; }

    private PulseQueueManager _pulseQueueManager { get; set; }

    private Dictionary<string, string> _currentState { get; set; }

    public long StartSequence(long count)
    {
        for (int i = 1; i <= count; i++)
        {
            _pulseQueueManager.Initialize();
        }
        return _pulseQueueManager.TotalPulses(); 
    }

    public string StartSequenceReturnState(long count)
    {
        var builder = new StringBuilder();

        var modules = FindAllFlipFlopSourcesOfRx();

        var lastIndexesPerModuleName = modules.ToDictionary(m => m.Name, m => -1); 
        var offsetsPerModuleName = modules.ToDictionary(m => m.Name, m => -1);
        var recurringsPerModuleName = modules.ToDictionary(m => m.Name, m => -1);
        var lastStates = modules.ToDictionary(m => m.Name, m => false);

        for (int i = 1; i <= count; i++)
        {
            _pulseQueueManager.Initialize();

            PopulateModuleStates(modules, lastIndexesPerModuleName, offsetsPerModuleName, recurringsPerModuleName,
                lastStates, i); 
        }

        foreach (var module in modules)
        {
            builder.AppendLine($"Module: {module.Name}, recur: {recurringsPerModuleName[module.Name]}, " +
                $"offset: {offsetsPerModuleName[module.Name]}"); 
        }

        return builder.ToString();
    }

    public string StartSequenceReturnStateNew(long count)
    {
        var builder = new StringBuilder();

        var modules = FindAllFlipFlopSourcesOfRx();

        var lastIndexesPerModuleName = modules.ToDictionary(m => m.Name, m => -1);
        var offsetsPerModuleName = modules.ToDictionary(m => m.Name, m => -1);
        var recurringsPerModuleName = modules.ToDictionary(m => m.Name, m => -1);
        var lastStates = modules.ToDictionary(m => m.Name, m => false);

        for (int i = 1; i <= count; i++)
        {
            _pulseQueueManager.Initialize();

            PopulateModuleStates(modules, lastIndexesPerModuleName, offsetsPerModuleName, recurringsPerModuleName,
                lastStates, i);
        }

        foreach (var module in modules)
        {
            builder.AppendLine($"Module: {module.Name}, recur: {recurringsPerModuleName[module.Name]}, " +
                $"offset: {offsetsPerModuleName[module.Name]}");
        }

        return builder.ToString();
    }

    private void PopulateModuleStates(List<Module> modules, Dictionary<string, int> lastIndexesPerModuleName,
        Dictionary<string, int> offsetsPerModuleName, Dictionary<string, int> recurringsPerModuleName, 
        Dictionary<string, bool> lastStates,
        int index)
    {
        foreach (var item in modules)
        {
            var flipFlop = item as FlipFlopModule; 
            var currentLastIndex = lastIndexesPerModuleName[item.Name];
            var currentOffset = offsetsPerModuleName[item.Name];
            var currentRecur = recurringsPerModuleName[item.Name];

            if (item.Name == "rj")
            {
                var a = 1; 
            }

            if (flipFlop.GetState() && !lastStates[item.Name])
            {
                //Recurrings
                if (currentLastIndex == -1)
                {
                    //do nothing for recur - next time
                }
                else if (currentRecur == -1)
                {
                    var newRecur = index - currentLastIndex;
                    recurringsPerModuleName[item.Name] = newRecur;
                }
                else if (currentRecur == -2)
                {
                    //these are set for non-recurring
                }
                else
                {
                    var newRecur = index - currentLastIndex;
                    if (currentRecur != newRecur)
                        recurringsPerModuleName[item.Name] = -2; 
                }

                if (currentOffset == -1)
                    offsetsPerModuleName[item.Name] = index;


                lastIndexesPerModuleName[item.Name] = index; 
            }
            lastStates[item.Name] = flipFlop.GetState();
        }
    }

    public long SendLowToRx()
    {
        var result = _pulseQueueManager.SendLowToRx(out var buttonPressCount);

        return buttonPressCount; 
    }

    public List<Module> FindAllFlipFlopSourcesOfRx()
    {
        var modules = new List<Module>();
        var sources = _modules["rx"].SourceModules
            .Where(m => m.ModuleType == ModuleType.FlipFlop || m.ModuleType == ModuleType.Conjunction).ToList();
        
        while (true)
        {
            if (!sources.Any())
                break; 
            
            var flipFlop = sources.Where(m => m.ModuleType == ModuleType.FlipFlop);

            modules.AddRange(flipFlop); 

            sources = sources.Where(m => m.ModuleType == ModuleType.Conjunction).SelectMany(mod => mod.SourceModules).ToList();
        }
        return modules; 
    }

    public string ReturnState()
    {
        var builder = new StringBuilder();

        var modules = _modules.Where(m => m.Value.ModuleType == ModuleType.FlipFlop).OrderBy(m => m.Key).ToList();

        return String.Join(' ', modules.Select(m => m.Value.ReturnState()).ToList()); 
    }
}

public class Pulse
{
    public Pulse(Module from, Module to, bool state)
    {
        From = from;
        To = to; 
        State = state;
    }
    
    public Module From { get; set; }
    public Module To { get; set; }

    public bool State { get; set; }
}

public enum ModuleType
{
    BroadCaster, Output, FlipFlop, Conjunction, Dummy
}
