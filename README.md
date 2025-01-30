# Munkaerőpiaci Keresőprogram

Ez a projekt egy munkaerőpiaci keresőprogramot valósít meg, amely lehetővé teszi a felhasználók számára az álláshirdetések keresését és böngészését. A rendszer három fő komponenst tartalmaz: backend, frontend és adatbázis.

## Részletes Leírás

### Backend
- **Technológia**: C# .NET API
- **Leírás**: A backend a rendszer központi logikáját valósítja meg, felelős az adatkezelésért, a felhasználói műveletekért és az API végpontok kezeléséért. Az ASP.NET Core keretrendszert használja az API létrehozásához.
- **Funkcionalitás**: Az API képes kezelni az álláshirdetések lekérdezését, létrehozását, frissítését és törlését, valamint a felhasználói interakciókat.

### Frontend
- **Technológia**: React.js, Tailwind CSS
- **Leírás**: A frontend React alkalmazás, amely lehetővé teszi az álláshirdetések böngészését és keresését. A Tailwind CSS-t használja az egyszerű és gyors stílusformázáshoz.
- **Funkcionalitás**: Az alkalmazás segít a felhasználóknak az álláshirdetések szűrésében, valamint az egyes hirdetések megtekintésében.

### Adatbázis
- **Technológia**: SQL
- **Leírás**: Az adatbázis tárolja az álláshirdetéseket, a felhasználói adatokat és más fontos információkat. Az SQL adatbázis biztosítja az adatok gyors és hatékony kezelését.

### Dokumentáció
A projekt dokumentációja két formában érhető el:
- **PPT (PowerPoint)**: Prezentáció a projekt céljairól, technológiáiról és fejlesztési folyamatairól.
- **Írott Dokumentáció**: A rendszer felépítését és technikai részleteit bemutató dokumentum.

## Telepítés

### Backend telepítése
1. Klónozza a projektet:
    ```bash
    git clone https://github.com/username/project-name.git
    ```
2. Navigáljon a backend mappájába:
    ```bash
    cd backend
    ```
3. Telepítse a szükséges csomagokat:
    ```bash
    dotnet restore
    ```
4. Futtassa az API-t:
    ```bash
    dotnet run
    ```

### Frontend telepítése
1. Navigáljon a frontend mappájába:
    ```bash
    cd frontend
    ```
2. Telepítse a szükséges csomagokat:
    ```bash
    npm install
    ```
3. Futtassa a React alkalmazást:
    ```bash
    npm start
    ```

### Adatbázis konfigurálása
1. Állítsa be az SQL adatbázist a szükséges kapcsolatokkal és táblákkal.
2. A backend automatikusan csatlakozik az adatbázishoz a konfigurációs fájlok alapján.

