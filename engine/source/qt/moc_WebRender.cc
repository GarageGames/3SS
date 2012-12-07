/****************************************************************************
** Meta object code from reading C++ file 'WebRender.h'
**
** Created: Fri Dec 7 14:43:31 2012
**      by: The Qt Meta Object Compiler version 63 (Qt 4.8.1)
**
** WARNING! All changes made in this file will be lost!
*****************************************************************************/

#include "WebRender.h"
#if !defined(Q_MOC_OUTPUT_REVISION)
#error "The header file 'WebRender.h' doesn't include <QObject>."
#elif Q_MOC_OUTPUT_REVISION != 63
#error "This file was generated using the moc from 4.8.1. It"
#error "cannot be used with the include files from this version of Qt."
#error "(The moc has changed too much.)"
#endif

QT_BEGIN_MOC_NAMESPACE
static const uint qt_meta_data_WebRender[] = {

 // content:
       6,       // revision
       0,       // classname
       0,    0, // classinfo
       6,   14, // methods
       0,    0, // properties
       0,    0, // enums/sets
       0,    0, // constructors
       0,       // flags
       0,       // signalCount

 // slots: signature, parameters, type, tag, flags
      11,   10,   10,   10, 0x0a,
      33,   29,   10,   10, 0x0a,
      58,   55,   10,   10, 0x0a,
      88,   82,   10,   10, 0x0a,
     127,  117,   10,   10, 0x0a,
     155,   29,   10,   10, 0x0a,

       0        // eod
};

static const char qt_meta_stringdata_WebRender[] = {
    "WebRender\0\0loadStartedSlot()\0url\0"
    "linkClickedSlot(QUrl)\0ok\0"
    "finishLoadingSlot(bool)\0reply\0"
    "finishedSlot(QNetworkReply*)\0dirtyRect\0"
    "repaintRequestedSlot(QRect)\0"
    "urlChangedSlot(QUrl)\0"
};

void WebRender::qt_static_metacall(QObject *_o, QMetaObject::Call _c, int _id, void **_a)
{
    if (_c == QMetaObject::InvokeMetaMethod) {
        Q_ASSERT(staticMetaObject.cast(_o));
        WebRender *_t = static_cast<WebRender *>(_o);
        switch (_id) {
        case 0: _t->loadStartedSlot(); break;
        case 1: _t->linkClickedSlot((*reinterpret_cast< const QUrl(*)>(_a[1]))); break;
        case 2: _t->finishLoadingSlot((*reinterpret_cast< bool(*)>(_a[1]))); break;
        case 3: _t->finishedSlot((*reinterpret_cast< QNetworkReply*(*)>(_a[1]))); break;
        case 4: _t->repaintRequestedSlot((*reinterpret_cast< const QRect(*)>(_a[1]))); break;
        case 5: _t->urlChangedSlot((*reinterpret_cast< const QUrl(*)>(_a[1]))); break;
        default: ;
        }
    }
}

const QMetaObjectExtraData WebRender::staticMetaObjectExtraData = {
    0,  qt_static_metacall 
};

const QMetaObject WebRender::staticMetaObject = {
    { &QWebView::staticMetaObject, qt_meta_stringdata_WebRender,
      qt_meta_data_WebRender, &staticMetaObjectExtraData }
};

#ifdef Q_NO_DATA_RELOCATION
const QMetaObject &WebRender::getStaticMetaObject() { return staticMetaObject; }
#endif //Q_NO_DATA_RELOCATION

const QMetaObject *WebRender::metaObject() const
{
    return QObject::d_ptr->metaObject ? QObject::d_ptr->metaObject : &staticMetaObject;
}

void *WebRender::qt_metacast(const char *_clname)
{
    if (!_clname) return 0;
    if (!strcmp(_clname, qt_meta_stringdata_WebRender))
        return static_cast<void*>(const_cast< WebRender*>(this));
    return QWebView::qt_metacast(_clname);
}

int WebRender::qt_metacall(QMetaObject::Call _c, int _id, void **_a)
{
    _id = QWebView::qt_metacall(_c, _id, _a);
    if (_id < 0)
        return _id;
    if (_c == QMetaObject::InvokeMetaMethod) {
        if (_id < 6)
            qt_static_metacall(this, _c, _id, _a);
        _id -= 6;
    }
    return _id;
}
QT_END_MOC_NAMESPACE
