var d = 1.344d;
var dM = -1.344d;
var dZ = 0.344d;
var dZM = -0.344d;
var z = 0d;

var forceSign = false;
string? suffix = null;

var g0F = forceSign
    ? $"+0{suffix}"
    : "G0";

var n1F = forceSign
    ? $"+#.0{suffix};-#.0{suffix};+0.0{suffix}"
    : $"F1{suffix}";
        
var n2F = forceSign
    ? $"+#.00{suffix};-#.00{suffix};+0.00{suffix}"
    : $"F2{suffix}";
        
var p1F = forceSign
    ? $"+#.0{suffix};-#.0{suffix}"
    : $"#.0{suffix}";
        
var p2F = forceSign
    ? $"+#.00{suffix};-#.00{suffix}"
    : $"#.00{suffix}";
        
var p2NF = forceSign
    ? $"+0.00{suffix};-0.00{suffix};"
    : $"0.00{suffix}";
        
// Console.WriteLine($"d: {d}\ng0F: {d.ToString(g0F)}; n1F: {d.ToString(n1F)}; n2F: {d.ToString(n2F)}; p1F: {d.ToString(p1F)}; p2F: {d.ToString(p2F)}; p2NF: {d.ToString(p2NF)}");
// Console.WriteLine($"dM: {dM}\ng0F: {dM.ToString(g0F)}; n1F: {dM.ToString(n1F)}; n2F: {dM.ToString(n2F)}; p1F: {dM.ToString(p1F)}; p2F: {dM.ToString(p2F)}; p2NF: {dM.ToString(p2NF)}");
// Console.WriteLine($"dZ: {dZ}\ng0F: {dZ.ToString(g0F)}; n1F: {dZ.ToString(n1F)}; n2F: {dZ.ToString(n2F)}; p1F: {dZ.ToString(p1F)}; p2F: {dZ.ToString(p2F)}; p2NF: {dZ.ToString(p2NF)}");
// Console.WriteLine($"dZM: {dZM}\ng0F: {dZM.ToString(g0F)}; n1F: {dZM.ToString(n1F)}; n2F: {dZM.ToString(n2F)}; p1F: {dZM.ToString(p1F)}; p2F: {dZM.ToString(p2F)}; p2NF: {dZM.ToString(p2NF)}");
// Console.WriteLine($"z: {z}\ng0F: {z.ToString(g0F)}; n1F: {z.ToString(n1F)}; n2F: {z.ToString(n2F)}; p1F: {z.ToString(p1F)}; p2F: {z.ToString(p2F)}; p2NF: {z.ToString(p2NF)}");
// Forced
// d: 1.344
// g0F: +1; n1F: +1.3; n2F: +1.34; p1F: +1.3; p2F: +1.34; p2NF: +1.34
// dM: -1.344
// g0F: -+1; n1F: -1.3; n2F: -1.34; p1F: -1.3; p2F: -1.34; p2NF: -1.34
// dZ: 0.344
// g0F: +0; n1F: +.3; n2F: +.34; p1F: +.3; p2F: +.34; p2NF: +0.34
// dZM: -0.344
// g0F: -+0; n1F: -.3; n2F: -.34; p1F: -.3; p2F: -.34; p2NF: -0.34
// z: 0
// g0F: +0; n1F: +0.0; n2F: +0.00; p1F: +.0; p2F: +.00; p2NF: +0.00
//
// d: 1.344
// g0F: 1.344; n1F: 1.3; n2F: 1.34; p1F: 1.3; p2F: 1.34; p2NF: 1.34
// dM: -1.344
// g0F: -1.344; n1F: -1.3; n2F: -1.34; p1F: -1.3; p2F: -1.34; p2NF: -1.34
// dZ: 0.344
// g0F: 0.344; n1F: 0.3; n2F: 0.34; p1F: .3; p2F: .34; p2NF: 0.34
// dZM: -0.344
// g0F: -0.344; n1F: -0.3; n2F: -0.34; p1F: -.3; p2F: -.34; p2NF: -0.34
// z: 0
// g0F: 0; n1F: 0.0; n2F: 0.00; p1F: .0; p2F: .00; p2NF: 0.00


        
// var IwsF = $"+#;-#;+0";
var IwsF = $"N0";
Console.WriteLine($"d: {d} [] IwsF: {d.ToString(IwsF)}");
Console.WriteLine($"dM: {dM} [] IwsF: {dM.ToString(IwsF)}");
Console.WriteLine($"dZ: {dZ} [] IwsF: {dZ.ToString(IwsF)}");
Console.WriteLine($"dZM: {dZM} [] IwsF: {dZM.ToString(IwsF)}");
Console.WriteLine($"z: {z} [] IwsF: {z.ToString(IwsF)}");