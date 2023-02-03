const url = require("url");
const path = require("path");
// const { ConnectionBuilder } = require("electron-cgi");

import { app, BrowserWindow } from "electron";

let window: BrowserWindow | null;

const createWindow = () => {
  window = new BrowserWindow({ width: 800, height: 600 });

  window.loadURL(
    url.format({
      pathname: path.join(__dirname, "index.html"),
      protocol: "file:",
      slashes: true
    })
  );

  window.on("closed", () => {
    window = null;
  });
};

app.on("ready", createWindow);

app.on("window-all-closed", () => {
  if (process.platform !== "darwin") {
    app.quit();
  }
});

app.on("activate", () => {
  if (window === null) {
    createWindow();
  }
});

process.on('unhandledRejection', function (err) {
  console.log(err);
});

// let connection = new ConnectionBuilder()
//   .connectTo("dotnet", "run", "--project", "C:/proj/Pauli/client/backend/FrontConsole")
//   .build();

// connection.onDisconnect = () => {
//   console.log("lost");
// };


// const sendGreeting = () => {
//   connection.send('greeting', 'John', (error: any, theGreeting: any) => {
//   if (error) {
//       console.log(error); //serialized exception from the .NET handler
//       return;
//   }

//   console.log(theGreeting); // will print "Hello John!"
// });
// }
// export default sendGreeting;
