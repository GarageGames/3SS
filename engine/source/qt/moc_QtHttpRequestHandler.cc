/****************************************************************************
** Meta object code from reading C++ file 'QtHttpRequestHandler.h'
**
** Created: Fri Dec 7 14:35:30 2012
**      by: The Qt Meta Object Compiler version 63 (Qt 4.8.1)
**
** WARNING! All changes made in this file will be lost!
*****************************************************************************/

#include "QtHttpRequestHandler.h"
#if !defined(Q_MOC_OUTPUT_REVISION)
#error "The header file 'QtHttpRequestHandler.h' doesn't include <QObject>."
#elif Q_MOC_OUTPUT_REVISION != 63
#error "This file was generated using the moc from 4.8.1. It"
#error "cannot be used with the include files from this version of Qt."
#error "(The moc has changed too much.)"
#endif

QT_BEGIN_MOC_NAMESPACE
static const uint qt_meta_data_QtHTTPRequestHandler[] = {

 // content:
       6,       // revision
       0,       // classname
       0,    0, // classinfo
       4,   14, // methods
       0,    0, // properties
       0,    0, // enums/sets
       0,    0, // constructors
       0,       // flags
       0,       // signalCount

 // slots: signature, parameters, type, tag, flags
      22,   21,   21,   21, 0x0a,
      37,   21,   21,   21, 0x0a,
      74,   53,   21,   21, 0x0a,
     117,  110,   21,   21, 0x0a,

       0        // eod
};

static const char qt_meta_stringdata_QtHTTPRequestHandler[] = {
    "QtHTTPRequestHandler\0\0finishedSlot()\0"
    "readyReadSlot()\0bytesRead,totalBytes\0"
    "downloadProgressSlot(qint64,qint64)\0"
    "errors\0sslErrorsSlot(QList<QSslError>)\0"
};

void QtHTTPRequestHandler::qt_static_metacall(QObject *_o, QMetaObject::Call _c, int _id, void **_a)
{
    if (_c == QMetaObject::InvokeMetaMethod) {
        Q_ASSERT(staticMetaObject.cast(_o));
        QtHTTPRequestHandler *_t = static_cast<QtHTTPRequestHandler *>(_o);
        switch (_id) {
        case 0: _t->finishedSlot(); break;
        case 1: _t->readyReadSlot(); break;
        case 2: _t->downloadProgressSlot((*reinterpret_cast< qint64(*)>(_a[1])),(*reinterpret_cast< qint64(*)>(_a[2]))); break;
        case 3: _t->sslErrorsSlot((*reinterpret_cast< const QList<QSslError>(*)>(_a[1]))); break;
        default: ;
        }
    }
}

const QMetaObjectExtraData QtHTTPRequestHandler::staticMetaObjectExtraData = {
    0,  qt_static_metacall 
};

const QMetaObject QtHTTPRequestHandler::staticMetaObject = {
    { &QObject::staticMetaObject, qt_meta_stringdata_QtHTTPRequestHandler,
      qt_meta_data_QtHTTPRequestHandler, &staticMetaObjectExtraData }
};

#ifdef Q_NO_DATA_RELOCATION
const QMetaObject &QtHTTPRequestHandler::getStaticMetaObject() { return staticMetaObject; }
#endif //Q_NO_DATA_RELOCATION

const QMetaObject *QtHTTPRequestHandler::metaObject() const
{
    return QObject::d_ptr->metaObject ? QObject::d_ptr->metaObject : &staticMetaObject;
}

void *QtHTTPRequestHandler::qt_metacast(const char *_clname)
{
    if (!_clname) return 0;
    if (!strcmp(_clname, qt_meta_stringdata_QtHTTPRequestHandler))
        return static_cast<void*>(const_cast< QtHTTPRequestHandler*>(this));
    return QObject::qt_metacast(_clname);
}

int QtHTTPRequestHandler::qt_metacall(QMetaObject::Call _c, int _id, void **_a)
{
    _id = QObject::qt_metacall(_c, _id, _a);
    if (_id < 0)
        return _id;
    if (_c == QMetaObject::InvokeMetaMethod) {
        if (_id < 4)
            qt_static_metacall(this, _c, _id, _a);
        _id -= 4;
    }
    return _id;
}
QT_END_MOC_NAMESPACE
