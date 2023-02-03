# Pauli Semi-P2P
The goal was for this project was to be decentralized and P2P but to recude complexity the goal will now be a semi decentralized project with a centralized server. To achieve a more decentralization feel in this project the centralized server should obviously be open source and as simple as possible.

The is still a work in progress and the server or client is not yet developed.

### The Server
This will be where all the peers are connected to and should only be one instance of. The MVP data about the peers is Id, Username, IP Address and Public key. Requests should always be signed with the peer's local private key thus act as authorization.

### The Client - Backend and frontend
The backend and frontend will be made as a desktop application using electron with react and .NET. The clients backend fetches data to other peers and should use .NET webapi as controllers with its local SQL(Lite?).
