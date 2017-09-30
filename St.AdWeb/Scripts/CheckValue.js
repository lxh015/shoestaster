function checkDouble(value) {
    try {
        var temp = parseFloat(value);
        if (typeof (temp) == "number")
            return true;
        else
            return false;
    } catch (e) {
        return false;
    }
}

function checkHasSQLLine(value) {
    var selectReg = new RegExp("select");
    var fromReg = new RegExp("from");
    var scriptReg = new RegExp("script");
    var groupReg = new RegExp("group");

    if (selectReg.test(value))
        return true;
    if (fromReg.test(value))
        return true;
    if (scriptReg.test(value))
        return true;
    if (groupReg.test(value))
        return true;

    return false;
}