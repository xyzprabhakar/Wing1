﻿$(function () {
    let webRootPath = localStorage.getItem('_rootpath');
    if (webRootPath == null) { webRootPath = ""; }
    let sideMenutemp = JSON.parse(sessionStorage.getItem('_SideMenu'));
    let currentApplication = sessionStorage.getItem('_CurrentApplication');
    if (currentApplication == null) {
        currentApplication = 1;
    }
    if (sideMenutemp == null) {
        let requestUrl = webRootPath + "/Home/GetMenu";// + currentApplication ;
        $.ajax({ url: requestUrl }).done(function (data) {
            sessionStorage.setItem('_SideMenu', JSON.stringify(data));
            fncAddSideMenu(data)
        }).fail(function (jqXHR, exception) {
            // Our error logic here
            var msg = '';
            if (jqXHR.status === 0) {
                msg = 'Not connect.\n Verify Network.';
            } else if (jqXHR.status == 404) {
                msg = 'Requested page not found. [404]';
            } else if (jqXHR.status == 500) {
                msg = 'Internal Server Error [500].';
            } else if (exception === 'parsererror') {
                msg = 'Requested JSON parse failed.';
            } else if (exception === 'timeout') {
                msg = 'Time out error.';
            } else if (exception === 'abort') {
                msg = 'Ajax request aborted.';
            } else {
                msg = 'Uncaught Error.\n' + jqXHR.responseText;
            }
            alert(msg);
        })
    }
    else {
        fncAddSideMenu(sideMenutemp)
    }


    function fncCreateMenuIcon(iconname) {
        var elemet = document.createElement("i");
        elemet.setAttribute("class", iconname);
        return elemet;
    }
    function fncCreateMenuItem(menuname, iconname, linkname, hastree, isactive) {
        let item = document.createElement("li");

        item.classList.add("nav-item");
        if (hastree) {
            item.classList.add("has-treeview");
        }

        let itemlink = document.createElement("a");
        itemlink.href = linkname;
        itemlink.classList.add("nav-link");
        if (isactive) { itemlink.classList.add("active"); }
        itemlink.appendChild(fncCreateMenuIcon(iconname));

        let itemTxt = document.createElement("p");
        itemTxt.textContent = menuname;
        if (hastree) {
            itemTxt.appendChild(fncCreateMenuIcon("fas fa-angle-left right"));
        }
        itemlink.appendChild(itemTxt);
        item.appendChild(itemlink);
        return item;
    }
    function fncAddSideMenu(datas) {
        let _documentId = 0;
        let _moduleId = 0;
        let _submoduleId = 0;
        try {
            _documentId = parseInt(document.getElementById("_currentDocument").value);
            _moduleId = parseInt(document.getElementById("_currentModule").value);
            _submoduleId = parseInt(document.getElementById("_currentSubModule").value);           
        } catch (ex) {
        }

        panelSideMenu = document.getElementById("panelSideMenu");
        panelSideMenu.innerHTML = '';
        var AllModules = datas._module;
        var AllSubModules = datas._subModule;
        var AllDocuments = datas._document;
        //Remove Unnecessary SubModules
        for (let i = AllSubModules.length-1; i >= 0; i--) {
            let isFound = false;
            for (let j = 0; j < AllDocuments.length; j++) {
                if (AllDocuments[j].subModuleId == AllSubModules[i].id) {
                    isFound = true;
                    j = AllDocuments.length;
                }
            }
            if (!isFound) {
                AllSubModules.splice(i, 1);
            }
        }
        
        //Remove Unnecessary Module
        for (let i = AllModules.length-1; i >= 0; i--) {
            let isFound = false;
            for (let j = 0; j < AllDocuments.length; j++) {
                if (AllDocuments[j].moduleId == AllModules[i].id) {
                    isFound = true;
                    j = AllDocuments.length;
                }
            }
            if (!isFound) {
                for (let j = 0; j < AllSubModules.length; j++) {
                    if (AllSubModules[j].moduleId == AllModules[i].id) {
                        isFound = true;
                        j = AllSubModules.length;
                    }
                }
            }
            if (!isFound) {
                AllModules.splice(i, 1);
            }
        }

        //First Add Those Documet Which have no module or sub module
        
        for (let i = 0; i < AllDocuments.length; i++) {            
            if (AllDocuments[i].moduleId == 0 && AllDocuments[i].subModuleId == 0) {                
                let item = fncCreateMenuItem(AllDocuments[i].name, AllDocuments[i].icon, AllDocuments[i].actionName, false, AllDocuments[i].id == _documentId);                
                panelSideMenu.appendChild(item);
            }
        }
        //Get Modules
        for (let i = 0; i < AllModules.length; i++) {
            
            let ModuleItem = fncCreateMenuItem(AllModules[i].name, AllModules[i].icon, "#", true, AllModules[i].id == _moduleId);
            let ModuleUL = document.createElement("ul");
            ModuleUL.setAttribute("class", "nav nav-treeview");
            ModuleItem.appendChild(ModuleUL);

            for (let j = 0; j < AllSubModules.length; j++) {
                if (AllSubModules[j].moduleId == AllModules[i].id) {
                   
                    let SubModuleItem = fncCreateMenuItem(AllSubModules[j].name, AllSubModules[j].icon, "#", true, AllSubModules[j].id == _submoduleId);
                    let SubModuleUL = document.createElement("ul");
                    SubModuleUL.setAttribute("class", "nav nav-treeview");
                    SubModuleItem.appendChild(SubModuleUL);
                    for (let k = 0; k < AllDocuments.length; k++) {
                        if (AllDocuments[k].subModuleId == AllSubModules[j].id) {
                            let item = fncCreateMenuItem(AllDocuments[k].name, AllDocuments[k].icon, AllDocuments[k].actionName, false, AllDocuments[k].id == _documentId);
                            SubModuleUL.appendChild(item);
                        }
                    }
                    
                    ModuleUL.appendChild(SubModuleItem);
                }
            }

            for (let k = 0; k < AllDocuments.length; k++) {
                if (AllDocuments[k].moduleId == AllModules[i].id) {
                    let item = fncCreateMenuItem(AllDocuments[k].name, AllDocuments[k].icon, AllDocuments[k].actionName, false, AllDocuments[k].id == _documentId);
                    ModuleUL.appendChild(item);
                }
            }
            
            panelSideMenu.appendChild(ModuleItem);
        }

    }


});