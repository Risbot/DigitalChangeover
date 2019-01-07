# DigitalChangeover

## Návod k instalaci

1. **Vytvoření databáze**

   - V Microsoft SQL Server Management Studio spusťte tyto scripty:
     - **DB.sql** (vytvoří databázi )
     - **initdata.sql** (doplní databázi základními daty)

2. **Publikace serverové častí**

   - Publikujte z Visual studia projekt **HA.HostWeb** do složky a vytvořte pro ni v IIS web pod protokolem **https**.
   - Je třeba nastavit certifikát, buď vytvořte certifikát s názvem **DCHCert** a ten nainstalujte,
nebo pokud již máte certifikát zvolte jej a v soubory **Web.config** změňte název certifikátu **DCHCert** na název vámi zvoleného certifikátu.
   - Změna názvu ve **Web.config** pokud mate certifikát pod jiným názvem než **DCHCert**:
   ![bez nazvu](https://user-images.githubusercontent.com/25490269/29241020-7aa192fe-7f71-11e7-9777-4ce59f4806f2.png)
   ![bez nazvu](https://user-images.githubusercontent.com/25490269/29241070-336307dc-7f72-11e7-9c44-06b2f56eb6f0.png)
   - Ve **Web.config** souboru je také potřeba nastavit správný connection string do databáze:
   ![bez nazvu](https://user-images.githubusercontent.com/25490269/29241078-7037b0ae-7f72-11e7-81d6-a83c73499935.png)
   - Vzor založení webu v IIS:
   ![bez nazvu](https://user-images.githubusercontent.com/25490269/29241123-7ce0bb56-7f73-11e7-92c1-4f86c7e4493c.png)
   - Funkčnost server je možné vyzkoušet přístupem na službu https://localhost:447/DataService.svc, 447 je port který jste nastavili v konfiguraci. Pokud je vše v pořádku měly byste vidět tuto stránku:
   ![bez nazvu](https://user-images.githubusercontent.com/25490269/29241165-a7304e3e-7f74-11e7-96e9-1e7e7bb5ec8a.png)

3. **Publikace klientské častí**

   - V projektu **HA.MVVMClient** Upravte nastaveni služeb na správné cesty na server:
   ![bez nazvu](https://user-images.githubusercontent.com/25490269/29241406-6b7336ae-7f79-11e7-8eb3-37e31e1bc2e5.png)
   - Zde nastavte správnou cestu na službu:
   ![bez nazvu](https://user-images.githubusercontent.com/25490269/29241269-7c800894-7f76-11e7-99a9-52fba3c2778a.png)
   - Opakujte tuto operaci pro všechny 3 služby:
      - https://localhost:447/DataService.svc
      - https://localhost:447/FullTextService.svc
      - https://localhost:447/Security.svc

4. **Publikace klienta**

   - V nastaveni projektu **HA.MVVMClient** nastavte správné url odkud si budou moci uživatele stahovat klientskou část (zelena část na obrázku). Take nastavte verzi klientské části (oranžová část na obrázky). A také umístěni kam se bude publikovat.
   ![10](https://user-images.githubusercontent.com/25490269/50790279-d3528e80-12be-11e9-91ab-10cffc21165f.png)
   - Vytvořte web pro klientskou část stejně jako pro server. Muže byt hostování i pod protokolem **http**. Pokud bude vše v pořádku tak při přístupů na web uvidíte toto:
   ![bez nazvu](https://user-images.githubusercontent.com/25490269/29241374-a4be4684-7f78-11e7-8571-81d5f95ee567.png)

5. **Založení prvního administrátorského učtu**

   - V projektu **UserCreator** nastavte v souboru **App.config** správný connection string do databáze a program spusťte. 
   ![bez nazvu](https://user-images.githubusercontent.com/25490269/29241557-6c228f98-7f7c-11e7-9c78-ee97599e7fe1.png)
