/****************************************************************************
** Meta object code from reading C++ file 'QtFileDownloader.h'
**
** Created: Fri Dec 7 14:43:31 2012
**      by: The Qt Meta Object Compiler version 63 (Qt 4.8.1)
**
** WARNING! All changes made in this file will be lost!
*****************************************************************************/

#include "QtFileDownloader.h"
#if !defined(Q_MOC_OUTPUT_REVISION)
#error "The header file 'QtFileDownloader.h' doesn't include <QObject>."
#elif Q_MOC_OUTPUT_REVISION != 63
#error "This file was generated using the moc from 4.8.1. It"
#error "cannot be used with the include files from this version of Qt."
#error "(The moc has changed too much.)"
#endif

QT_BEGIN_MOC_NAMESPACE
static const uint qt_meta_data_QtFileDownloader[] = {

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
      18,   17,   17,   17, 0x0a,
      41,   17,   17,   17, 0x0a,
      82,   61,   17,   17, 0x0a,
     125,  118,   17,   17, 0x0a,

       0        // eod
};

static const char qt_meta_stringdata_QtFileDownloader[] = {
    "QtFileDownloader\0\0downloadFinishedSlot()\0"
    "dataReadyReadSlot()\0bytesRead,totalBytes\0"
    "downloadProgressSlot(qint64,qint64)\0"
    "errors\0sslErrorsSlot(QList<QSslError>)\0"
};

void QtFileDownloader::qt_static_metacall(QObject *_o, QMetaObject::Call _c, int _id, void **_a)
{
    if (_c == QMetaObject::InvokeMetaMethod) {
        Q_ASSERT(staticMetaObject.cast(_o));
        QtFileDownloader *_t = static_cast<QtFileDownloader *>(_o);
        switch (_id) {
        case 0: _t->downloadFinishedSlot(); break;
        case 1: _t->dataReadyReadSlot(); break;
        case 2: _t->downloadProgressSlot((*reinterpret_cast< qint64(*)>(_a[1])),(*reinterpret_cast< qint64(*)>(_a[2]))); break;
        case 3: _t->sslErrorsSlot((*reinterpret_cast< const QList<QSslError>(*)>(_a[1]))); break;
        default: ;
        }
    }
}

const QMetaObjectExtraData QtFileDownloader::staticMetaObjectExtraData = {
    0,  qt_static_metacall 
};

const QMetaObject QtFileDownloader::staticMetaObject = {
    { &QObject::staticMetaObject, qt_meta_stringdata_QtFileDownloader,
      qt_meta_data_QtFileDownloader, &staticMetaObjectExtraData }
};

#ifdef Q_NO_DATA_RELOCATION
const QMetaObject &QtFileDownloader::getStaticMetaObject() { return staticMetaObject; }
#endif //Q_NO_DATA_RELOCATION

const QMetaObject *QtFileDownloader::metaObject() const
{
    return QObject::d_ptr->metaObject ? QObject::d_ptr->metaObject : &staticMetaObject;
}

void *QtFileDownloader::qt_metacast(const char *_clname)
{
    if (!_clname) return 0;
    if (!strcmp(_clname, qt_meta_stringdata_QtFileDownloader))
        return static_cast<void*>(const_cast< QtFileDownloader*>(this));
    return QObject::qt_metacast(_clname);
}

int QtFileDownloader::qt_metacall(QMetaObject::Call _c, int _id, void **_a)
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
