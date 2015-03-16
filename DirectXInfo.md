# DirectX #

Za multimedijalne elemente programa u C# moze se koristiti DirectX 9 u Managed izvedbi.

## Instalacija ##
  1. Potreban je instalisan Microsoft Visual C# ( npr. verzija _2008 Express Edition_)
  1. Instalisati Microsoft DirectX Software Development Kit - za download verzije _Novembar 2008_ videti **[link ](http://www.microsoft.com/downloads/details.aspx?familyid=5493F76A-6D37-478D-BA17-28B1CCA4865A&displaylang=en)** ( ~480 MB )

## Podesavanje ##

Za koriscenje DirectX-a u C# projektima neophodno je :
  1. Kreirati nov projekat
  1. Dodati neophodne DirectX reference u projekat preko glavnog menija : Project -> Add Reference -> tab .NET -> izabrati MicrosoftDirectX i potrebne MicrosoftDirectX.XYZ dll-ove (koristiti samo verzije 1.0.2902.0) -> OK
  1. U .cs fajlovima gde se koristi DirectX dodati _using_ direktive, npr. using Microsoft.DirectX; ...
  1. Sink your teeth into it

## Linkovi ##
  * [MSDN DirectX 9 Managed](http://msdn.microsoft.com/en-us/library/ms804954.aspx) - zvanicna strana, ali sadrzaj vremenom nestaje misteriozno, pozurite
  * [Riemer's DirectX Tutorial](http://www.riemers.net/eng/Tutorials/DirectX/Csharp/series1.php) - postupno objasnjeni elementi DirectX-a, praceni celokupnim kodom u C#
  * [Drunken Hyena](http://www.drunkenhyena.com/cgi-bin/dx9_net.pl) - osnove i primeri u C# i VB.Net
  * [TheZBuffer](http://www.thezbuffer.com/categories/tutorials.aspx) - lista ManagedDirectX tutorijala, primera i (polu)gotovih projekata
  * [CodeSampler](http://www.codesampler.com/dx9src.htm) - Direct3D primeri u C++ i C#
  * [CodeProject](http://www.codeproject.com/KB/directx/) - Direct Draw, DirectX igre i DirectX uopste

## Napomene ##
  * Direct Draw is deprecated
  * Potencijalni problem sa Loader Lock se moze resiti ( osim metodom ponovnog pokusaja ) tako sto se u glavnom meniju : Debug -> Exceptions -> u grupi _Managed Debugging Assistants_ iskljuci Loader Lock stavka
  * Pojedini konflikti koji se javljaju u toku kodiranja, a vezani su za _ambiguously defined_ gresku i preklapanje namespace-ova, ako ne sprecavaju Build Solution, se (jedino) mogu ignorisati