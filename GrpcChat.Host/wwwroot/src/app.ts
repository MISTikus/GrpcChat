// tslint:disable

import { AuthenticationPromiseClient } from "./proto/Services/auth_grpc_web_pb";
// import { RoomPromiseClient } from "./proto/Services/room_grpc_web_pb";
import { Credentials } from "./proto/Models/models_pb";


const credentials = new Credentials();

credentials.setUsername("Admin");
credentials.setPassword("admin");

const authClient = new AuthenticationPromiseClient("http://localhost:50001", {}, null);

authClient
  .authenticate(credentials)
  .then(console.log)
  .catch(console.error)

