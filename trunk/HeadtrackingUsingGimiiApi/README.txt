Osnovne smernice za pokretanje primera.

Za pokretanje je potrebno imati instaliran DirectXSDK November 2008.
Link za download: http://www.microsoft.com/downloads/details.aspx?familyid=5493F76A-6D37-478D-BA17-28B1CCA4865A&displaylang=en

Pre pokretanja upariti wii kontroler preko bluetooth veze i odmah nakon uparivanja pokrenuti KeepWii.exe, koji se nalazi u istoimenom projektu u okviru repozitorijuma, da se ne bi naprasno diskonektovao.

Ovaj solution sadrzi 2 projekta. Otvara se dvoklikom na "HeadtrackingUsingGimiiApi.sln". U solution exploreru desni klik na projekat koji se zeli pokrenuti prilikom debagiranja i odabrati opciju "Set as StartUp Project" (naziv odabranog projekta postaje boldovan). U source kodu je pre pokretanja potrebno podesiti rezoluciju ekrana, visinu u milimetrima, kao i da li se wii kontroler nalazi iznad ili ispod ekrana. Podesavanja se u oba projekta nalaze na pocetku klase Primer1.
Ukoliko se pri pokretanju javi greska u kojoj se pominje "LoaderLock" odabrati u meniju Debug>Exceptions>Managed Debugging Assistans i iskljuciti checkbox pored linije LoaderLock.