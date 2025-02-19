﻿var db = null;
var openRequest = indexedDB.open("dpbs", 1);
openRequest.onupgradeneeded = function (e) {
    console.log("Upgrading...");
    var thisDB = e.target.result;
    if (!thisDB.objectStoreNames.contains("tblCountryMaster")) {
        thisDB.createObjectStore("tblCountryMaster", { keyPath: "countryId" });
    }
    if (!thisDB.objectStoreNames.contains("tblStateMaster")) {
        var objectStore = thisDB.createObjectStore("tblStateMaster", { keyPath: "stateId" });
        objectStore.createIndex("countryId", "countryId", { unique: false });
    }
    
}

function LoadDataInDB(urls, postingdata, tableName) {
    fetch(urls, {
        method: "POST",
        credentials: 'same-origin',
        headers: {
            "Content-Type": "application/json; charset=utf-8"
        },
        body: postingdata
    }).then(response => response.json())
        .then(data => {
            let ObjectStore = db.transaction(tableName, "readwrite")
                .objectStore(tableName);
            let ReqSuccess = ObjectStore.clear();
            ReqSuccess.onsuccess = function (event) {
                for (var i in data) {
                    ObjectStore.add(data[i]);
                }
                console.log(tableName.substring(3));
                postMessage("Loaded " + tableName.substring(3));
            };
        });
}


openRequest.onsuccess = function (e) {
    console.log("Success!");
    db = e.target.result;
    postMessage("Loading data...");
    //delete exiting data
    LoadDataInDB("/api/Returnapi/GetCountry", {}, 'tblCountryMaster');
    LoadDataInDB("/api/Returnapi/GetState", {}, 'tblStateMaster');
    
}
openRequest.onerror = function (e) {
    console.log("Error");
    console.dir(e);
}

