(function () {
    /*
 *****************************************************************************
Copyright (c) Microsoft Corporation. All rights reserved.
Licensed under the Apache License, Version 2.0 (the "License"); you may not use
this file except in compliance with the License. You may obtain a copy of the
License at http://www.apache.org/licenses/LICENSE-2.0

THIS CODE IS PROVIDED ON AN *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED
WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
MERCHANTABLITY OR NON-INFRINGEMENT.

See the Apache Version 2.0 License for specific language governing permissions
and limitations under the License.
*****************************************************************************/
    var $jscomp = $jscomp || {};
    $jscomp.scope = {};
    $jscomp.arrayIteratorImpl = function (d) {
        var l = 0;
        return function () {
            return l < d.length ? { done: !1, value: d[l++] } : { done: !0 };
        };
    };
    $jscomp.arrayIterator = function (d) {
        return { next: $jscomp.arrayIteratorImpl(d) };
    };
    $jscomp.makeIterator = function (d) {
        var l = "undefined" != typeof Symbol && Symbol.iterator && d[Symbol.iterator];
        return l ? l.call(d) : $jscomp.arrayIterator(d);
    };
    $jscomp.getGlobal = function (d) {
        return "undefined" != typeof window && window === d ? d : "undefined" != typeof global && null != global ? global : d;
    };
    $jscomp.global = $jscomp.getGlobal(this);
    $jscomp.ASSUME_ES5 = !1;
    $jscomp.ASSUME_NO_NATIVE_MAP = !1;
    $jscomp.ASSUME_NO_NATIVE_SET = !1;
    $jscomp.SIMPLE_FROUND_POLYFILL = !1;
    $jscomp.defineProperty =
        $jscomp.ASSUME_ES5 || "function" == typeof Object.defineProperties
            ? Object.defineProperty
            : function (d, l, g) {
                d != Array.prototype && d != Object.prototype && (d[l] = g.value);
            };
    $jscomp.polyfill = function (d, l, g, f) {
        if (l) {
            g = $jscomp.global;
            d = d.split(".");
            for (f = 0; f < d.length - 1; f++) {
                var h = d[f];
                h in g || (g[h] = {});
                g = g[h];
            }
            d = d[d.length - 1];
            f = g[d];
            l = l(f);
            l != f && null != l && $jscomp.defineProperty(g, d, { configurable: !0, writable: !0, value: l });
        }
    };
    $jscomp.FORCE_POLYFILL_PROMISE = !1;
    $jscomp.polyfill(
        "Promise",
        function (d) {
            function l() {
                this.batch_ = null;
            }
            function g(b) {
                return b instanceof h
                    ? b
                    : new h(function (a, c) {
                        a(b);
                    });
            }
            if (d && !$jscomp.FORCE_POLYFILL_PROMISE) return d;
            l.prototype.asyncExecute = function (b) {
                null == this.batch_ && ((this.batch_ = []), this.asyncExecuteBatch_());
                this.batch_.push(b);
                return this;
            };
            l.prototype.asyncExecuteBatch_ = function () {
                var b = this;
                this.asyncExecuteFunction(function () {
                    b.executeBatch_();
                });
            };
            var f = $jscomp.global.setTimeout;
            l.prototype.asyncExecuteFunction = function (b) {
                f(b, 0);
            };
            l.prototype.executeBatch_ = function () {
                for (; this.batch_ && this.batch_.length;) {
                    var b = this.batch_;
                    this.batch_ = [];
                    for (var a = 0; a < b.length; ++a) {
                        var c = b[a];
                        b[a] = null;
                        try {
                            c();
                        } catch (m) {
                            this.asyncThrow_(m);
                        }
                    }
                }
                this.batch_ = null;
            };
            l.prototype.asyncThrow_ = function (b) {
                this.asyncExecuteFunction(function () {
                    throw b;
                });
            };
            var h = function (b) {
                this.state_ = 0;
                this.result_ = void 0;
                this.onSettledCallbacks_ = [];
                var a = this.createResolveAndReject_();
                try {
                    b(a.resolve, a.reject);
                } catch (c) {
                    a.reject(c);
                }
            };
            h.prototype.createResolveAndReject_ = function () {
                function b(m) {
                    return function (b) {
                        c || ((c = !0), m.call(a, b));
                    };
                }
                var a = this,
                    c = !1;
                return { resolve: b(this.resolveTo_), reject: b(this.reject_) };
            };
            h.prototype.resolveTo_ = function (b) {
                if (b === this) this.reject_(new TypeError("A Promise cannot resolve to itself"));
                else if (b instanceof h) this.settleSameAsPromise_(b);
                else {
                    a: switch (typeof b) {
                        case "object":
                            var a = null != b;
                            break a;
                        case "function":
                            a = !0;
                            break a;
                        default:
                            a = !1;
                    }
                    a ? this.resolveToNonPromiseObj_(b) : this.fulfill_(b);
                }
            };
            h.prototype.resolveToNonPromiseObj_ = function (b) {
                var a = void 0;
                try {
                    a = b.then;
                } catch (c) {
                    this.reject_(c);
                    return;
                }
                "function" == typeof a ? this.settleSameAsThenable_(a, b) : this.fulfill_(b);
            };
            h.prototype.reject_ = function (b) {
                this.settle_(2, b);
            };
            h.prototype.fulfill_ = function (b) {
                this.settle_(1, b);
            };
            h.prototype.settle_ = function (b, a) {
                if (0 != this.state_) throw Error("Cannot settle(" + b + ", " + a + "): Promise already settled in state" + this.state_);
                this.state_ = b;
                this.result_ = a;
                this.executeOnSettledCallbacks_();
            };
            h.prototype.executeOnSettledCallbacks_ = function () {
                if (null != this.onSettledCallbacks_) {
                    for (var f = 0; f < this.onSettledCallbacks_.length; ++f) b.asyncExecute(this.onSettledCallbacks_[f]);
                    this.onSettledCallbacks_ = null;
                }
            };
            var b = new l();
            h.prototype.settleSameAsPromise_ = function (b) {
                var a = this.createResolveAndReject_();
                b.callWhenSettled_(a.resolve, a.reject);
            };
            h.prototype.settleSameAsThenable_ = function (b, a) {
                var c = this.createResolveAndReject_();
                try {
                    b.call(a, c.resolve, c.reject);
                } catch (m) {
                    c.reject(m);
                }
            };
            h.prototype.then = function (b, a) {
                function c(a, c) {
                    return "function" == typeof a
                        ? function (c) {
                            try {
                                m(a(c));
                            } catch (u) {
                                p(u);
                            }
                        }
                        : c;
                }
                var m,
                    p,
                    x = new h(function (a, c) {
                        m = a;
                        p = c;
                    });
                this.callWhenSettled_(c(b, m), c(a, p));
                return x;
            };
            h.prototype.catch = function (b) {
                return this.then(void 0, b);
            };
            h.prototype.callWhenSettled_ = function (f, a) {
                function c() {
                    switch (m.state_) {
                        case 1:
                            f(m.result_);
                            break;
                        case 2:
                            a(m.result_);
                            break;
                        default:
                            throw Error("Unexpected state: " + m.state_);
                    }
                }
                var m = this;
                null == this.onSettledCallbacks_ ? b.asyncExecute(c) : this.onSettledCallbacks_.push(c);
            };
            h.resolve = g;
            h.reject = function (b) {
                return new h(function (a, c) {
                    c(b);
                });
            };
            h.race = function (b) {
                return new h(function (a, c) {
                    for (var m = $jscomp.makeIterator(b), p = m.next(); !p.done; p = m.next()) g(p.value).callWhenSettled_(a, c);
                });
            };
            h.all = function (b) {
                var a = $jscomp.makeIterator(b),
                    c = a.next();
                return c.done
                    ? g([])
                    : new h(function (m, b) {
                        function p(a) {
                            return function (c) {
                                n[a] = c;
                                f--;
                                0 == f && m(n);
                            };
                        }
                        var n = [],
                            f = 0;
                        do n.push(void 0), f++, g(c.value).callWhenSettled_(p(n.length - 1), b), (c = a.next());
                        while (!c.done);
                    });
            };
            return h;
        },
        "es6",
        "es3"
    );
    $jscomp.checkStringArgs = function (d, l, g) {
        if (null == d) throw new TypeError("The 'this' value for String.prototype." + g + " must not be null or undefined");
        if (l instanceof RegExp) throw new TypeError("First argument to String.prototype." + g + " must not be a regular expression");
        return d + "";
    };
    $jscomp.polyfill(
        "String.prototype.endsWith",
        function (d) {
            return d
                ? d
                : function (d, g) {
                    var f = $jscomp.checkStringArgs(this, d, "endsWith");
                    d += "";
                    void 0 === g && (g = f.length);
                    g = Math.max(0, Math.min(g | 0, f.length));
                    for (var h = d.length; 0 < h && 0 < g;) if (f[--g] != d[--h]) return !1;
                    return 0 >= h;
                };
        },
        "es6",
        "es3"
    );
    $jscomp.checkEs6ConformanceViaProxy = function () {
        try {
            var d = {},
                l = Object.create(
                    new $jscomp.global.Proxy(d, {
                        get: function (g, f, h) {
                            return g == d && "q" == f && h == l;
                        },
                    })
                );
            return !0 === l.q;
        } catch (g) {
            return !1;
        }
    };
    $jscomp.USE_PROXY_FOR_ES6_CONFORMANCE_CHECKS = !1;
    $jscomp.ES6_CONFORMANCE = $jscomp.USE_PROXY_FOR_ES6_CONFORMANCE_CHECKS && $jscomp.checkEs6ConformanceViaProxy();
    $jscomp.SYMBOL_PREFIX = "jscomp_symbol_";
    $jscomp.initSymbol = function () {
        $jscomp.initSymbol = function () { };
        $jscomp.global.Symbol || ($jscomp.global.Symbol = $jscomp.Symbol);
    };
    $jscomp.Symbol = (function () {
        var d = 0;
        return function (l) {
            return $jscomp.SYMBOL_PREFIX + (l || "") + d++;
        };
    })();
    $jscomp.initSymbolIterator = function () {
        $jscomp.initSymbol();
        var d = $jscomp.global.Symbol.iterator;
        d || (d = $jscomp.global.Symbol.iterator = $jscomp.global.Symbol("iterator"));
        "function" != typeof Array.prototype[d] &&
            $jscomp.defineProperty(Array.prototype, d, {
                configurable: !0,
                writable: !0,
                value: function () {
                    return $jscomp.iteratorPrototype($jscomp.arrayIteratorImpl(this));
                },
            });
        $jscomp.initSymbolIterator = function () { };
    };
    $jscomp.initSymbolAsyncIterator = function () {
        $jscomp.initSymbol();
        var d = $jscomp.global.Symbol.asyncIterator;
        d || (d = $jscomp.global.Symbol.asyncIterator = $jscomp.global.Symbol("asyncIterator"));
        $jscomp.initSymbolAsyncIterator = function () { };
    };
    $jscomp.iteratorPrototype = function (d) {
        $jscomp.initSymbolIterator();
        d = { next: d };
        d[$jscomp.global.Symbol.iterator] = function () {
            return this;
        };
        return d;
    };
    $jscomp.owns = function (d, l) {
        return Object.prototype.hasOwnProperty.call(d, l);
    };
    $jscomp.polyfill(
        "WeakMap",
        function (d) {
            function l() {
                if (!d || !Object.seal) return !1;
                try {
                    var a = Object.seal({}),
                        m = Object.seal({}),
                        b = new d([
                            [a, 2],
                            [m, 3],
                        ]);
                    if (2 != b.get(a) || 3 != b.get(m)) return !1;
                    b.delete(a);
                    b.set(m, 4);
                    return !b.has(a) && 4 == b.get(m);
                } catch (x) {
                    return !1;
                }
            }
            function g() { }
            function f(a) {
                if (!$jscomp.owns(a, b)) {
                    var c = new g();
                    $jscomp.defineProperty(a, b, { value: c });
                }
            }
            function h(a) {
                var c = Object[a];
                c &&
                    (Object[a] = function (a) {
                        if (a instanceof g) return a;
                        f(a);
                        return c(a);
                    });
            }
            if ($jscomp.USE_PROXY_FOR_ES6_CONFORMANCE_CHECKS) {
                if (d && $jscomp.ES6_CONFORMANCE) return d;
            } else if (l()) return d;
            var b = "$jscomp_hidden_" + Math.random();
            h("freeze");
            h("preventExtensions");
            h("seal");
            var t = 0,
                a = function (a) {
                    this.id_ = (t += Math.random() + 1).toString();
                    if (a) {
                        a = $jscomp.makeIterator(a);
                        for (var c; !(c = a.next()).done;) (c = c.value), this.set(c[0], c[1]);
                    }
                };
            a.prototype.set = function (a, m) {
                f(a);
                if (!$jscomp.owns(a, b)) throw Error("WeakMap key fail: " + a);
                a[b][this.id_] = m;
                return this;
            };
            a.prototype.get = function (a) {
                return $jscomp.owns(a, b) ? a[b][this.id_] : void 0;
            };
            a.prototype.has = function (a) {
                return $jscomp.owns(a, b) && $jscomp.owns(a[b], this.id_);
            };
            a.prototype.delete = function (a) {
                return $jscomp.owns(a, b) && $jscomp.owns(a[b], this.id_) ? delete a[b][this.id_] : !1;
            };
            return a;
        },
        "es6",
        "es3"
    );
    $jscomp.MapEntry = function () { };
    $jscomp.polyfill(
        "Map",
        function (d) {
            function l() {
                if ($jscomp.ASSUME_NO_NATIVE_MAP || !d || "function" != typeof d || !d.prototype.entries || "function" != typeof Object.seal) return !1;
                try {
                    var a = Object.seal({ x: 4 }),
                        b = new d($jscomp.makeIterator([[a, "s"]]));
                    if ("s" != b.get(a) || 1 != b.size || b.get({ x: 4 }) || b.set({ x: 4 }, "t") != b || 2 != b.size) return !1;
                    var p = b.entries(),
                        x = p.next();
                    if (x.done || x.value[0] != a || "s" != x.value[1]) return !1;
                    x = p.next();
                    return x.done || 4 != x.value[0].x || "t" != x.value[1] || !p.next().done ? !1 : !0;
                } catch (n) {
                    return !1;
                }
            }
            if ($jscomp.USE_PROXY_FOR_ES6_CONFORMANCE_CHECKS) {
                if (d && $jscomp.ES6_CONFORMANCE) return d;
            } else if (l()) return d;
            $jscomp.initSymbolIterator();
            var g = new WeakMap(),
                f = function (a) {
                    this.data_ = {};
                    this.head_ = t();
                    this.size = 0;
                    if (a) {
                        a = $jscomp.makeIterator(a);
                        for (var c; !(c = a.next()).done;) (c = c.value), this.set(c[0], c[1]);
                    }
                };
            f.prototype.set = function (a, b) {
                a = 0 === a ? 0 : a;
                var c = h(this, a);
                c.list || (c.list = this.data_[c.id] = []);
                c.entry
                    ? (c.entry.value = b)
                    : ((c.entry = { next: this.head_, previous: this.head_.previous, head: this.head_, key: a, value: b }), c.list.push(c.entry), (this.head_.previous.next = c.entry), (this.head_.previous = c.entry), this.size++);
                return this;
            };
            f.prototype.delete = function (a) {
                a = h(this, a);
                return a.entry && a.list
                    ? (a.list.splice(a.index, 1), a.list.length || delete this.data_[a.id], (a.entry.previous.next = a.entry.next), (a.entry.next.previous = a.entry.previous), (a.entry.head = null), this.size--, !0)
                    : !1;
            };
            f.prototype.clear = function () {
                this.data_ = {};
                this.head_ = this.head_.previous = t();
                this.size = 0;
            };
            f.prototype.has = function (a) {
                return !!h(this, a).entry;
            };
            f.prototype.get = function (a) {
                return (a = h(this, a).entry) && a.value;
            };
            f.prototype.entries = function () {
                return b(this, function (a) {
                    return [a.key, a.value];
                });
            };
            f.prototype.keys = function () {
                return b(this, function (a) {
                    return a.key;
                });
            };
            f.prototype.values = function () {
                return b(this, function (a) {
                    return a.value;
                });
            };
            f.prototype.forEach = function (a, b) {
                for (var c = this.entries(), m; !(m = c.next()).done;) (m = m.value), a.call(b, m[1], m[0], this);
            };
            f.prototype[Symbol.iterator] = f.prototype.entries;
            var h = function (c, b) {
                var m = b && typeof b;
                "object" == m || "function" == m ? (g.has(b) ? (m = g.get(b)) : ((m = "" + ++a), g.set(b, m))) : (m = "p_" + b);
                var f = c.data_[m];
                if (f && $jscomp.owns(c.data_, m))
                    for (c = 0; c < f.length; c++) {
                        var n = f[c];
                        if ((b !== b && n.key !== n.key) || b === n.key) return { id: m, list: f, index: c, entry: n };
                    }
                return { id: m, list: f, index: -1, entry: void 0 };
            },
                b = function (a, b) {
                    var c = a.head_;
                    return $jscomp.iteratorPrototype(function () {
                        if (c) {
                            for (; c.head != a.head_;) c = c.previous;
                            for (; c.next != c.head;) return (c = c.next), { done: !1, value: b(c) };
                            c = null;
                        }
                        return { done: !0, value: void 0 };
                    });
                },
                t = function () {
                    var a = {};
                    return (a.previous = a.next = a.head = a);
                },
                a = 0;
            return f;
        },
        "es6",
        "es3"
    );
    $jscomp.findInternal = function (d, l, g) {
        d instanceof String && (d = String(d));
        for (var f = d.length, h = 0; h < f; h++) {
            var b = d[h];
            if (l.call(g, b, h, d)) return { i: h, v: b };
        }
        return { i: -1, v: void 0 };
    };
    $jscomp.polyfill(
        "Array.prototype.find",
        function (d) {
            return d
                ? d
                : function (d, g) {
                    return $jscomp.findInternal(this, d, g).v;
                };
        },
        "es6",
        "es3"
    );
    $jscomp.underscoreProtoCanBeSet = function () {
        var d = { a: !0 },
            l = {};
        try {
            return (l.__proto__ = d), l.a;
        } catch (g) { }
        return !1;
    };
    $jscomp.setPrototypeOf =
        "function" == typeof Object.setPrototypeOf
            ? Object.setPrototypeOf
            : $jscomp.underscoreProtoCanBeSet()
                ? function (d, l) {
                    d.__proto__ = l;
                    if (d.__proto__ !== l) throw new TypeError(d + " is not extensible");
                    return d;
                }
                : null;
    $jscomp.polyfill(
        "Object.setPrototypeOf",
        function (d) {
            return d || $jscomp.setPrototypeOf;
        },
        "es6",
        "es5"
    );
    $jscomp.assign =
        "function" == typeof Object.assign
            ? Object.assign
            : function (d, l) {
                for (var g = 1; g < arguments.length; g++) {
                    var f = arguments[g];
                    if (f) for (var h in f) $jscomp.owns(f, h) && (d[h] = f[h]);
                }
                return d;
            };
    $jscomp.polyfill(
        "Object.assign",
        function (d) {
            return d || $jscomp.assign;
        },
        "es6",
        "es3"
    );
    $jscomp.iteratorFromArray = function (d, l) {
        $jscomp.initSymbolIterator();
        d instanceof String && (d += "");
        var g = 0,
            f = {
                next: function () {
                    if (g < d.length) {
                        var h = g++;
                        return { value: l(h, d[h]), done: !1 };
                    }
                    f.next = function () {
                        return { done: !0, value: void 0 };
                    };
                    return f.next();
                },
            };
        f[Symbol.iterator] = function () {
            return f;
        };
        return f;
    };
    $jscomp.polyfill(
        "Array.prototype.keys",
        function (d) {
            return d
                ? d
                : function () {
                    return $jscomp.iteratorFromArray(this, function (d) {
                        return d;
                    });
                };
        },
        "es6",
        "es3"
    );
    (function (d) {
        function l(f) {
            if (g[f]) return g[f].exports;
            var h = (g[f] = { i: f, l: !1, exports: {} });
            d[f].call(h.exports, h, h.exports, l);
            h.l = !0;
            return h.exports;
        }
        var g = {};
        l.m = d;
        l.c = g;
        l.d = function (f, h, b) {
            l.o(f, h) || Object.defineProperty(f, h, { enumerable: !0, get: b });
        };
        l.r = function (f) {
            "undefined" !== typeof Symbol && Symbol.toStringTag && Object.defineProperty(f, Symbol.toStringTag, { value: "Module" });
            Object.defineProperty(f, "__esModule", { value: !0 });
        };
        l.t = function (f, h) {
            h & 1 && (f = l(f));
            if (h & 8 || (h & 4 && "object" === typeof f && f && f.__esModule)) return f;
            var b = Object.create(null);
            l.r(b);
            Object.defineProperty(b, "default", { enumerable: !0, value: f });
            if (h & 2 && "string" != typeof f)
                for (var d in f)
                    l.d(
                        b,
                        d,
                        function (a) {
                            return f[a];
                        }.bind(null, d)
                    );
            return b;
        };
        l.n = function (f) {
            var h =
                f && f.__esModule
                    ? function () {
                        return f["default"];
                    }
                    : function () {
                        return f;
                    };
            l.d(h, "a", h);
            return h;
        };
        l.o = function (f, h) {
            return Object.prototype.hasOwnProperty.call(f, h);
        };
        l.p = "/core/pdf/";
        return l((l.s = 17));
    })([
        function (d, l, g) {
            g.d(l, "d", function () {
                return t;
            });
            g.d(l, "e", function () {
                return h;
            });
            g.d(l, "c", function () {
                return a;
            });
            g.d(l, "a", function () {
                return c;
            });
            g.d(l, "b", function () {
                return b;
            });
            var f = g(3),
                h = function (a, c) {
                    Object(f.a)("disableLogs") || (c ? console.warn(a + ": " + c) : console.warn(a));
                },
                b = function (a, c, b) {
                    var m = b.pop();
                    b = b.length ? b.join(", ") + " and " + m : m;
                    h("'" + c + "' is deprecated since version " + a + ". Please use " + b + " instead.");
                },
                t = function (a, c) {
                    Object(f.a)("disableLogs") || (c ? console.log(a + ": " + c) : console.log(a));
                },
                a = function (a) {
                    if (!Object(f.a)("disableLogs")) throw (console.error(a), Error(a));
                },
                c = function (a, c) { };
        },
        function (d, l, g) {
            g.d(l, "c", function () {
                return c;
            });
            g.d(l, "a", function () {
                return m;
            });
            g.d(l, "b", function () {
                return p;
            });
            g.d(l, "d", function () {
                return x;
            });
            var f = g(9),
                h = console.log,
                b = console.warn,
                t = console.error,
                a = function (a) {
                    void 0 === a && (a = !0);
                    a ? ((console.log = function () { }), (console.warn = function () { }), (console.error = function () { })) : ((console.log = h), (console.warn = b), (console.error = t));
                },
                c = function () {
                    var c = Object(f.a)(location.search);
                    a("1" === c.disableLogs);
                },
                m = function (c) {
                    c.on("disableLogs", function (c) {
                        a(c.disabled);
                    });
                },
                p = function (a, c) {
                    return function () { };
                },
                x = function (a, c) {
                    c ? console.warn(a + ": " + c) : console.warn(a);
                };
        },
        function (d, l, g) {
            g.d(l, "a", function () {
                return z;
            });
            g.d(l, "b", function () {
                return q;
            });
            g.d(l, "c", function () {
                return k;
            });
            var f = g(12),
                h = g(0),
                b = g(5),
                t = g(4),
                a = "undefined" === typeof window ? self : window,
                c = a.importScripts,
                m = !1,
                p = function (e, k) {
                    m || (c(a.basePath + "decode.min.js"), (m = !0));
                    e = self.BrotliDecode(Object(t.b)(e));
                    return k ? e : Object(t.a)(e);
                },
                x = function (e, k) {
                    return Object(f.a)(void 0, void 0, Promise, function () {
                        var a;
                        return Object(f.b)(this, function (q) {
                            switch (q.label) {
                                case 0:
                                    return m ? [3, 2] : [4, Object(b.b)(self.Core.getWorkerPath() + "external/decode.min.js", "Failed to download decode.min.js", window)];
                                case 1:
                                    q.sent(), (m = !0), (q.label = 2);
                                case 2:
                                    return (a = self.BrotliDecode(Object(t.b)(e))), [2, k ? a : Object(t.a)(a)];
                            }
                        });
                    });
                };
            (function () {
                function e() {
                    this.remainingDataArrays = [];
                }
                e.prototype.processRaw = function (e) {
                    return e;
                };
                e.prototype.processBrotli = function (e) {
                    this.remainingDataArrays.push(e);
                    return null;
                };
                e.prototype.GetNextChunk = function (e) {
                    this.decodeFunction || (this.decodeFunction = 0 === e[0] && 97 === e[1] && 115 === e[2] && 109 === e[3] ? this.processRaw : this.processBrotli);
                    return this.decodeFunction(e);
                };
                e.prototype.End = function () {
                    if (this.remainingDataArrays.length) {
                        for (var e = this.arrays, k = 0, a = 0; a < e.length; ++a) k += e[a].length;
                        k = new Uint8Array(k);
                        var q = 0;
                        for (a = 0; a < e.length; ++a) {
                            var c = e[a];
                            k.set(c, q);
                            q += c.length;
                        }
                        return p(k, !0);
                    }
                    return null;
                };
                return e;
            })();
            var n = !1,
                w = function (e) {
                    n || (c(a.basePath + "pako_inflate.min.js"), (n = !0));
                    var k = 10;
                    if ("string" === typeof e) {
                        if (e.charCodeAt(3) & 8) {
                            for (; 0 !== e.charCodeAt(k); ++k);
                            ++k;
                        }
                    } else if (e[3] & 8) {
                        for (; 0 !== e[k]; ++k);
                        ++k;
                    }
                    e = Object(t.b)(e);
                    e = e.subarray(k, e.length - 8);
                    return a.pako.inflate(e, { windowBits: -15 });
                },
                y = function (e, k) {
                    return k ? e : Object(t.a)(e);
                },
                u = function (e) {
                    var k = !e.shouldOutputArray,
                        a = new XMLHttpRequest();
                    a.open("GET", e.url, e.isAsync);
                    var q = k && a.overrideMimeType;
                    a.responseType = q ? "text" : "arraybuffer";
                    q && a.overrideMimeType("text/plain; charset=x-user-defined");
                    a.send();
                    var b = function () {
                        var b = Date.now();
                        var v = q ? a.responseText : new Uint8Array(a.response);
                        Object(h.a)("worker", "Result length is " + v.length);
                        v.length < e.compressedMaximum
                            ? ((v = e.decompressFunction(v, e.shouldOutputArray)),
                                Object(h.e)(
                                    "There may be some degradation of performance. Your server has not been configured to serve .gz. and .br. files with the expected Content-Encoding. See http://www.pdftron.com/kb_content_encoding for instructions on how to resolve this."
                                ),
                                c && Object(h.a)("worker", "Decompressed length is " + v.length))
                            : k && (v = Object(t.a)(v));
                        c && Object(h.a)("worker", e.url + " Decompression took " + (Date.now() - b));
                        return v;
                    };
                    if (e.isAsync)
                        var B = new Promise(function (k, q) {
                            a.onload = function () {
                                200 === this.status || 0 === this.status ? k(b()) : q("Download Failed " + e.url);
                            };
                            a.onerror = function () {
                                q("Network error occurred " + e.url);
                            };
                        });
                    else {
                        if (200 === a.status || 0 === a.status) return b();
                        throw Error("Failed to load " + e.url);
                    }
                    return B;
                },
                z = function (e) {
                    var k = e.lastIndexOf("/");
                    -1 === k && (k = 0);
                    var a = e.slice(k).replace(".", ".br.");
                    c || (a.endsWith(".js.mem") ? (a = a.replace(".js.mem", ".mem")) : a.endsWith(".js") && (a = a.concat(".mem")));
                    return e.slice(0, k) + a;
                },
                A = function (e, k) {
                    var a = e.lastIndexOf("/");
                    -1 === a && (a = 0);
                    var q = e.slice(a).replace(".", ".gz.");
                    k.url = e.slice(0, a) + q;
                    k.decompressFunction = w;
                    return u(k);
                },
                r = function (e, k) {
                    k.url = z(e);
                    k.decompressFunction = c ? p : x;
                    return u(k);
                },
                C = function (e, k) {
                    e.endsWith(".js.mem") ? (e = e.slice(0, -4)) : e.endsWith(".mem") && (e = e.slice(0, -4) + ".js.mem");
                    k.url = e;
                    k.decompressFunction = y;
                    return u(k);
                },
                H = function (e, k, a, q) {
                    return e.catch(function (e) {
                        Object(h.e)(e);
                        return q(k, a);
                    });
                },
                e = function (e, k, a) {
                    var q;
                    if (a.isAsync) {
                        var c = k[0](e, a);
                        for (q = 1; q < k.length; ++q) c = H(c, e, a, k[q]);
                        return c;
                    }
                    for (q = 0; q < k.length; ++q)
                        try {
                            return k[q](e, a);
                        } catch (N) {
                            Object(h.e)(N.message);
                        }
                    throw Error("");
                },
                k = function (k, a, q, c) {
                    return e(k, [A, r, C], { compressedMaximum: a, isAsync: q, shouldOutputArray: c });
                },
                q = function (k, a, q, c) {
                    return e(k, [r, A, C], { compressedMaximum: a, isAsync: q, shouldOutputArray: c });
                };
        },
        function (d, l, g) {
            g.d(l, "a", function () {
                return b;
            });
            g.d(l, "b", function () {
                return t;
            });
            var f = {},
                h = { flattenedResources: !1, CANVAS_CACHE_SIZE: void 0, maxPagesBefore: void 0, maxPagesAhead: void 0, disableLogs: !1, wvsQueryParameters: {}, _trnDebugMode: !1, _logFiltersEnabled: null },
                b = function (a) {
                    return h[a];
                },
                t = function (a, c) {
                    var b;
                    h[a] = c;
                    null === (b = f[a]) || void 0 === b
                        ? void 0
                        : b.forEach(function (a) {
                            a(c);
                        });
                };
        },
        function (d, l, g) {
            g.d(l, "b", function () {
                return f;
            });
            g.d(l, "a", function () {
                return h;
            });
            var f = function (b) {
                if ("string" === typeof b) {
                    for (var f = new Uint8Array(b.length), a = b.length, c = 0; c < a; c++) f[c] = b.charCodeAt(c);
                    return f;
                }
                return b;
            },
                h = function (b) {
                    if ("string" !== typeof b) {
                        for (var f = "", a = 0, c = b.length, m; a < c;) (m = b.subarray(a, a + 1024)), (a += 1024), (f += String.fromCharCode.apply(null, m));
                        return f;
                    }
                    return b;
                };
        },
        function (d, l, g) {
            function f(a, c, m) {
                function f(d) {
                    n = n || Date.now();
                    return d
                        ? (Object(b.a)("load", "Try instantiateStreaming"),
                            fetch(Object(t.a)(a))
                                .then(function (a) {
                                    return WebAssembly.instantiateStreaming(a, c);
                                })
                                .catch(function (c) {
                                    Object(b.a)("load", "instantiateStreaming Failed " + a + " message " + c.message);
                                    return f(!1);
                                }))
                        : Object(t.b)(a, m, !0, !0).then(function (a) {
                            h = Date.now();
                            Object(b.a)("load", "Request took " + (h - n) + " ms");
                            return WebAssembly.instantiate(a, c);
                        });
                }
                var h, n;
                return f(!!WebAssembly.instantiateStreaming).then(function (a) {
                    Object(b.a)("load", "WASM compilation took " + (Date.now() - (h || n)) + " ms");
                    return a;
                });
            }
            function h(a, c, m) {
                return new Promise(function (f) {
                    if (!a) return f();
                    var h = m.document.createElement("script");
                    h.type = "text/javascript";
                    h.onload = function () {
                        f();
                    };
                    h.onerror = function () {
                        c && Object(b.e)(c);
                        f();
                    };
                    h.src = a;
                    m.document.getElementsByTagName("head")[0].appendChild(h);
                });
            }
            g.d(l, "a", function () {
                return f;
            });
            g.d(l, "b", function () {
                return h;
            });
            var b = g(0),
                t = g(2);
        },
        function (d, l, g) {
            g.d(l, "c", function () {
                return p;
            });
            g.d(l, "b", function () {
                return x;
            });
            g.d(l, "a", function () {
                return n;
            });
            g(0);
            var f = "undefined" === typeof window ? self : window;
            d = (function () {
                var a = navigator.userAgent.toLowerCase();
                return (a = /(msie) ([\w.]+)/.exec(a) || /(trident)(?:.*? rv:([\w.]+)|)/.exec(a)) ? parseInt(a[2], 10) : a;
            })();
            var h = (function () {
                var a = f.navigator.userAgent.match(/OPR/),
                    c = f.navigator.userAgent.match(/Maxthon/),
                    b = f.navigator.userAgent.match(/Edge/);
                return f.navigator.userAgent.match(/Chrome\/(.*?) /) && !a && !c && !b;
            })();
            (function () {
                if (!h) return null;
                var a = f.navigator.userAgent.match(/Chrome\/([0-9]+)\./);
                return a ? parseInt(a[1], 10) : a;
            })();
            var b = !!navigator.userAgent.match(/Edge/i) || (navigator.userAgent.match(/Edg\/(.*?)/) && f.navigator.userAgent.match(/Chrome\/(.*?) /));
            (function () {
                if (!b) return null;
                var a = f.navigator.userAgent.match(/Edg\/([0-9]+)\./);
                return a ? parseInt(a[1], 10) : a;
            })();
            l = /iPad|iPhone|iPod/.test(f.navigator.platform) || ("MacIntel" === navigator.platform && 1 < navigator.maxTouchPoints);
            var t = (function () {
                var a = f.navigator.userAgent.match(/.*\/([0-9\.]+)\s(Safari|Mobile).*/i);
                return a ? parseFloat(a[1]) : a;
            })(),
                a = /^((?!chrome|android).)*safari/i.test(f.navigator.userAgent) || (/^((?!chrome|android).)*$/.test(f.navigator.userAgent) && l),
                c = f.navigator.userAgent.match(/Firefox/);
            (function () {
                if (!c) return null;
                var a = f.navigator.userAgent.match(/Firefox\/([0-9]+)\./);
                return a ? parseInt(a[1], 10) : a;
            })();
            d || /Android|webOS|Touch|IEMobile|Silk/i.test(navigator.userAgent);
            navigator.userAgent.match(/(iPad|iPhone|iPod)/i);
            f.navigator.userAgent.indexOf("Android");
            var m = /Mac OS X 10_13_6.*\(KHTML, like Gecko\)$/.test(f.navigator.userAgent),
                p = function () {
                    return (a && 14 > t) || m;
                },
                x = !(!self.WebAssembly || !self.WebAssembly.validate),
                n = -1 < f.navigator.userAgent.indexOf("Edge/16") || -1 < f.navigator.userAgent.indexOf("MSAppHost");
        },
        function (d, l, g) {
            function f(a) {
                f =
                    "function" === typeof Symbol && "symbol" === typeof Symbol.iterator
                        ? function (a) {
                            return typeof a;
                        }
                        : function (a) {
                            return a && "function" === typeof Symbol && a.constructor === Symbol && a !== Symbol.prototype ? "symbol" : typeof a;
                        };
                return f(a);
            }
            var h, b, t;
            !(function (a) {
                "object" === f(l) && "undefined" !== typeof d ? (d.exports = a()) : !((b = []), (h = a), (t = "function" === typeof h ? h.apply(l, b) : h), void 0 !== t && (d.exports = t));
            })(function () {
                return (function x(c, b, f) {
                    function m(n, d) {
                        if (!b[n]) {
                            if (!c[n]) {
                                if (h) return h(n, !0);
                                d = Error("Cannot find module '".concat(n, "'"));
                                throw ((d.code = "MODULE_NOT_FOUND"), d);
                            }
                            d = b[n] = { exports: {} };
                            c[n][0].call(
                                d.exports,
                                function (b) {
                                    return m(c[n][1][b] || b);
                                },
                                d,
                                d.exports,
                                x,
                                c,
                                b,
                                f
                            );
                        }
                        return b[n].exports;
                    }
                    for (var h = !1, d = 0; d < f.length; d++) m(f[d]);
                    return m;
                })(
                    {
                        1: [
                            function (c, b, f) {
                                var m = {}.hasOwnProperty,
                                    n = function (c, b) {
                                        function f() {
                                            this.constructor = c;
                                        }
                                        for (var n in b) m.call(b, n) && (c[n] = b[n]);
                                        f.prototype = b.prototype;
                                        c.prototype = new f();
                                        c.__super__ = b.prototype;
                                        return c;
                                    };
                                f = c("./PriorityQueue/AbstractPriorityQueue");
                                c = c("./PriorityQueue/ArrayStrategy");
                                f = (function (c) {
                                    function b(c) {
                                        c || (c = {});
                                        c.strategy || (c.strategy = BinaryHeapStrategy);
                                        c.comparator ||
                                            (c.comparator = function (c, b) {
                                                return (c || 0) - (b || 0);
                                            });
                                        b.__super__.constructor.call(this, c);
                                    }
                                    n(b, c);
                                    return b;
                                })(f);
                                f.ArrayStrategy = c;
                                b.exports = f;
                            },
                            { "./PriorityQueue/AbstractPriorityQueue": 2, "./PriorityQueue/ArrayStrategy": 3 },
                        ],
                        2: [
                            function (c, b, f) {
                                b.exports = (function () {
                                    function c(c) {
                                        if (null == (null != c ? c.strategy : void 0)) throw "Must pass options.strategy, a strategy";
                                        if (null == (null != c ? c.comparator : void 0)) throw "Must pass options.comparator, a comparator";
                                        this.priv = new c.strategy(c);
                                        this.length = 0;
                                    }
                                    c.prototype.queue = function (c) {
                                        this.length++;
                                        this.priv.queue(c);
                                    };
                                    c.prototype.dequeue = function (c) {
                                        if (!this.length) throw "Empty queue";
                                        this.length--;
                                        return this.priv.dequeue();
                                    };
                                    c.prototype.peek = function (c) {
                                        if (!this.length) throw "Empty queue";
                                        return this.priv.peek();
                                    };
                                    c.prototype.remove = function (c) {
                                        this.priv.remove(c) && --this.length;
                                    };
                                    c.prototype.find = function (c) {
                                        return 0 <= this.priv.find(c);
                                    };
                                    c.prototype.removeAllMatching = function (c, b) {
                                        c = this.priv.removeAllMatching(c, b);
                                        this.length -= c;
                                    };
                                    return c;
                                })();
                            },
                            {},
                        ],
                        3: [
                            function (c, b, f) {
                                var m = function (c, b, m) {
                                    var f;
                                    var d = 0;
                                    for (f = c.length; d < f;) {
                                        var h = (d + f) >>> 1;
                                        0 <= m(c[h], b) ? (d = h + 1) : (f = h);
                                    }
                                    return d;
                                };
                                b.exports = (function () {
                                    function c(c) {
                                        var b;
                                        this.options = c;
                                        this.comparator = this.options.comparator;
                                        this.data = (null != (b = this.options.initialValues) ? b.slice(0) : void 0) || [];
                                        this.data.sort(this.comparator).reverse();
                                    }
                                    c.prototype.queue = function (c) {
                                        var b = m(this.data, c, this.comparator);
                                        this.data.splice(b, 0, c);
                                    };
                                    c.prototype.dequeue = function () {
                                        return this.data.pop();
                                    };
                                    c.prototype.peek = function () {
                                        return this.data[this.data.length - 1];
                                    };
                                    c.prototype.find = function (c) {
                                        var b = m(this.data, c, this.comparator) - 1;
                                        return 0 <= b && !this.comparator(this.data[b], c) ? b : -1;
                                    };
                                    c.prototype.remove = function (c) {
                                        c = this.find(c);
                                        return 0 <= c ? (this.data.splice(c, 1), !0) : !1;
                                    };
                                    c.prototype.removeAllMatching = function (c, b) {
                                        for (var m = 0, f = this.data.length - 1; 0 <= f; --f)
                                            if (c(this.data[f])) {
                                                var d = this.data.splice(f, 1)[0];
                                                b && b(d);
                                                ++m;
                                            }
                                        return m;
                                    };
                                    return c;
                                })();
                            },
                            {},
                        ],
                    },
                    {},
                    [1]
                )(1);
            });
        },
        function (d, l, g) {
            (function (f) {
                function d(a, c) {
                    this._id = a;
                    this._clearFn = c;
                }
                var b = ("undefined" !== typeof f && f) || ("undefined" !== typeof self && self) || window,
                    t = Function.prototype.apply;
                l.setTimeout = function () {
                    return new d(t.call(setTimeout, b, arguments), clearTimeout);
                };
                l.setInterval = function () {
                    return new d(t.call(setInterval, b, arguments), clearInterval);
                };
                l.clearTimeout = l.clearInterval = function (a) {
                    a && a.close();
                };
                d.prototype.unref = d.prototype.ref = function () { };
                d.prototype.close = function () {
                    this._clearFn.call(b, this._id);
                };
                l.enroll = function (a, c) {
                    clearTimeout(a._idleTimeoutId);
                    a._idleTimeout = c;
                };
                l.unenroll = function (a) {
                    clearTimeout(a._idleTimeoutId);
                    a._idleTimeout = -1;
                };
                l._unrefActive = l.active = function (a) {
                    clearTimeout(a._idleTimeoutId);
                    var c = a._idleTimeout;
                    0 <= c &&
                        (a._idleTimeoutId = setTimeout(function () {
                            a._onTimeout && a._onTimeout();
                        }, c));
                };
                g(21);
                l.setImmediate = ("undefined" !== typeof self && self.setImmediate) || ("undefined" !== typeof f && f.setImmediate) || (this && this.setImmediate);
                l.clearImmediate = ("undefined" !== typeof self && self.clearImmediate) || ("undefined" !== typeof f && f.clearImmediate) || (this && this.clearImmediate);
            }.call(this, g(10)));
        },
        function (d, l, g) {
            l.a = function (f) {
                var d = {};
                decodeURIComponent(f.slice(1))
                    .split("&")
                    .forEach(function (b) {
                        b = b.split("=", 2);
                        d[b[0]] = b[1];
                    });
                return d;
            };
        },
        function (d, l) {
            l = (function () {
                return this;
            })();
            try {
                l = l || new Function("return this")();
            } catch (g) {
                "object" === typeof window && (l = window);
            }
            d.exports = l;
        },
        function (d, l, g) {
            g.d(l, "b", function () {
                return t;
            });
            g.d(l, "a", function () {
                return c;
            });
            var f = g(2),
                h = g(5),
                b = g(6),
                t = function () {
                    return b.b && !b.a && !Object(b.c)();
                },
                a = (function () {
                    function a(a) {
                        var c = this;
                        this.promise = a.then(function (a) {
                            c.response = a;
                            c.status = 200;
                        });
                    }
                    a.prototype.addEventListener = function (a, c) {
                        this.promise.then(c);
                    };
                    return a;
                })(),
                c = function (c, b, d) {
                    if (t() && !d)
                        (self.Module.instantiateWasm = function (a, m) {
                            return Object(h.a)(c + "Wasm.wasm", a, b["Wasm.wasm"]).then(function (a) {
                                m(a.instance);
                            });
                        }),
                            (d = Object(f.b)(c + "Wasm.js.mem", b["Wasm.js.mem"], !1, !1));
                    else {
                        d = Object(f.b)((self.Module.asmjsPrefix ? self.Module.asmjsPrefix : "") + c + ".js.mem", b[".js.mem"], !1);
                        var m = Object(f.c)((self.Module.memoryInitializerPrefixURL ? self.Module.memoryInitializerPrefixURL : "") + c + ".mem", b[".mem"], !0, !0);
                        self.Module.memoryInitializerRequest = new a(m);
                    }
                    d = new Blob([d], { type: "application/javascript" });
                    importScripts(URL.createObjectURL(d));
                };
        },
        function (d, l, g) {
            function f(b, f, a, c) {
                return new (a || (a = Promise))(function (m, d) {
                    function h(a) {
                        try {
                            p(c.next(a));
                        } catch (u) {
                            d(u);
                        }
                    }
                    function n(a) {
                        try {
                            p(c["throw"](a));
                        } catch (u) {
                            d(u);
                        }
                    }
                    function p(c) {
                        c.done
                            ? m(c.value)
                            : new a(function (a) {
                                a(c.value);
                            }).then(h, n);
                    }
                    p((c = c.apply(b, f || [])).next());
                });
            }
            function h(b, f) {
                function a(a) {
                    return function (b) {
                        return c([a, b]);
                    };
                }
                function c(a) {
                    if (d) throw new TypeError("Generator is already executing.");
                    for (; m;)
                        try {
                            if (((d = 1), h && (n = a[0] & 2 ? h["return"] : a[0] ? h["throw"] || ((n = h["return"]) && n.call(h), 0) : h.next) && !(n = n.call(h, a[1])).done)) return n;
                            if (((h = 0), n)) a = [a[0] & 2, n.value];
                            switch (a[0]) {
                                case 0:
                                case 1:
                                    n = a;
                                    break;
                                case 4:
                                    return m.label++, { value: a[1], done: !1 };
                                case 5:
                                    m.label++;
                                    h = a[1];
                                    a = [0];
                                    continue;
                                case 7:
                                    a = m.ops.pop();
                                    m.trys.pop();
                                    continue;
                                default:
                                    if (!((n = m.trys), (n = 0 < n.length && n[n.length - 1])) && (6 === a[0] || 2 === a[0])) {
                                        m = 0;
                                        continue;
                                    }
                                    if (3 === a[0] && (!n || (a[1] > n[0] && a[1] < n[3]))) m.label = a[1];
                                    else if (6 === a[0] && m.label < n[1]) (m.label = n[1]), (n = a);
                                    else if (n && m.label < n[2]) (m.label = n[2]), m.ops.push(a);
                                    else {
                                        n[2] && m.ops.pop();
                                        m.trys.pop();
                                        continue;
                                    }
                            }
                            a = f.call(b, m);
                        } catch (u) {
                            (a = [6, u]), (h = 0);
                        } finally {
                            d = n = 0;
                        }
                    if (a[0] & 5) throw a[1];
                    return { value: a[0] ? a[1] : void 0, done: !0 };
                }
                var m = {
                    label: 0,
                    sent: function () {
                        if (n[0] & 1) throw n[1];
                        return n[1];
                    },
                    trys: [],
                    ops: [],
                },
                    d,
                    h,
                    n,
                    g;
                return (
                    (g = { next: a(0), throw: a(1), return: a(2) }),
                    "function" === typeof Symbol &&
                    (g[Symbol.iterator] = function () {
                        return this;
                    }),
                    g
                );
            }
            g.d(l, "a", function () {
                return f;
            });
            g.d(l, "b", function () {
                return h;
            });
        },
        function (d, l) {
            function g() {
                throw Error("setTimeout has not been defined");
            }
            function f() {
                throw Error("clearTimeout has not been defined");
            }
            function h(a) {
                if (p === setTimeout) return setTimeout(a, 0);
                if ((p === g || !p) && setTimeout) return (p = setTimeout), setTimeout(a, 0);
                try {
                    return p(a, 0);
                } catch (A) {
                    try {
                        return p.call(null, a, 0);
                    } catch (r) {
                        return p.call(this, a, 0);
                    }
                }
            }
            function b(a) {
                if (x === clearTimeout) return clearTimeout(a);
                if ((x === f || !x) && clearTimeout) return (x = clearTimeout), clearTimeout(a);
                try {
                    return x(a);
                } catch (A) {
                    try {
                        return x.call(null, a);
                    } catch (r) {
                        return x.call(this, a);
                    }
                }
            }
            function t() {
                w && y && ((w = !1), y.length ? (n = y.concat(n)) : (u = -1), n.length && a());
            }
            function a() {
                if (!w) {
                    var a = h(t);
                    w = !0;
                    for (var c = n.length; c;) {
                        y = n;
                        for (n = []; ++u < c;) y && y[u].run();
                        u = -1;
                        c = n.length;
                    }
                    y = null;
                    w = !1;
                    b(a);
                }
            }
            function c(a, c) {
                this.fun = a;
                this.array = c;
            }
            function m() { }
            d = d.exports = {};
            try {
                var p = "function" === typeof setTimeout ? setTimeout : g;
            } catch (z) {
                p = g;
            }
            try {
                var x = "function" === typeof clearTimeout ? clearTimeout : f;
            } catch (z) {
                x = f;
            }
            var n = [],
                w = !1,
                y,
                u = -1;
            d.nextTick = function (b) {
                var m = Array(arguments.length - 1);
                if (1 < arguments.length) for (var f = 1; f < arguments.length; f++) m[f - 1] = arguments[f];
                n.push(new c(b, m));
                1 !== n.length || w || h(a);
            };
            c.prototype.run = function () {
                this.fun.apply(null, this.array);
            };
            d.title = "browser";
            d.browser = !0;
            d.env = {};
            d.argv = [];
            d.version = "";
            d.versions = {};
            d.on = m;
            d.addListener = m;
            d.once = m;
            d.off = m;
            d.removeListener = m;
            d.removeAllListeners = m;
            d.emit = m;
            d.prependListener = m;
            d.prependOnceListener = m;
            d.listeners = function (a) {
                return [];
            };
            d.binding = function (a) {
                throw Error("process.binding is not supported");
            };
            d.cwd = function () {
                return "/";
            };
            d.chdir = function (a) {
                throw Error("process.chdir is not supported");
            };
            d.umask = function () {
                return 0;
            };
        },
        function (d, l, g) {
            l.a = function () {
                ArrayBuffer.prototype.slice ||
                    (ArrayBuffer.prototype.slice = function (f, d) {
                        void 0 === f && (f = 0);
                        void 0 === d && (d = this.byteLength);
                        f = Math.floor(f);
                        d = Math.floor(d);
                        0 > f && (f += this.byteLength);
                        0 > d && (d += this.byteLength);
                        f = Math.min(Math.max(0, f), this.byteLength);
                        d = Math.min(Math.max(0, d), this.byteLength);
                        if (0 >= d - f) return new ArrayBuffer(0);
                        var b = new ArrayBuffer(d - f),
                            h = new Uint8Array(b);
                        f = new Uint8Array(this, f, d - f);
                        h.set(f);
                        return b;
                    });
            };
        },
        function (d, l, g) {
            (function (f) {
                function d(a) {
                    "function" !== typeof a && (a = new Function("" + a));
                    for (var c = Array(arguments.length - 1), e = 0; e < c.length; e++) c[e] = arguments[e + 1];
                    y[w] = { callback: a, args: c };
                    A(w);
                    return w++;
                }
                function b(a) {
                    delete y[a];
                }
                function g(a) {
                    if (u) setTimeout(g, 0, a);
                    else {
                        var c = y[a];
                        if (c) {
                            u = !0;
                            try {
                                var e = c.callback,
                                    k = c.args;
                                switch (k.length) {
                                    case 0:
                                        e();
                                        break;
                                    case 1:
                                        e(k[0]);
                                        break;
                                    case 2:
                                        e(k[0], k[1]);
                                        break;
                                    case 3:
                                        e(k[0], k[1], k[2]);
                                        break;
                                    default:
                                        e.apply(void 0, k);
                                }
                            } finally {
                                b(a), (u = !1);
                            }
                        }
                    }
                }
                function a() {
                    A = function (a) {
                        f.nextTick(function () {
                            g(a);
                        });
                    };
                }
                function c() {
                    if (n.postMessage && !n.importScripts) {
                        var a = !0,
                            c = n.onmessage;
                        n.onmessage = function () {
                            a = !1;
                        };
                        n.postMessage("", "*");
                        n.onmessage = c;
                        return a;
                    }
                }
                function m() {
                    var a = "setImmediate$" + Math.random() + "$",
                        c = function (e) {
                            (e.source !== n && e.source !== n.parent) || "string" !== typeof e.data || 0 !== e.data.indexOf(a) || g(+e.data.slice(a.length));
                        };
                    n.addEventListener ? n.addEventListener("message", c, !1) : n.attachEvent("onmessage", c);
                    A = function (e) {
                        n.postMessage(a + e, "*");
                    };
                }
                function p() {
                    var a = z.documentElement;
                    A = function (c) {
                        var e = z.createElement("script");
                        e.onreadystatechange = function () {
                            g(c);
                            e.onreadystatechange = null;
                            a.removeChild(e);
                            e = null;
                        };
                        a.appendChild(e);
                    };
                }
                function x() {
                    A = function (a) {
                        setTimeout(g, 0, a);
                    };
                }
                var n = "undefined" === typeof window ? self : window,
                    w = 1,
                    y = {},
                    u = !1,
                    z = n.document,
                    A,
                    r = Object.getPrototypeOf && Object.getPrototypeOf(n);
                r = r && r.setTimeout ? r : n;
                "[object process]" === {}.toString.call(n.process) ? a() : c() ? m() : z && "onreadystatechange" in z.createElement("script") ? p() : x();
                r.setImmediate = d;
                r.clearImmediate = b;
                l.a = { setImmediate: d, clearImmediate: b };
            }.call(this, g(13)));
        },
        function (d, l, g) {
            var f = g(0);
            d = (function () {
                function d(b, d) {
                    this.name = b;
                    this.comObj = d;
                    this.callbackIndex = 1;
                    this.postMessageTransfers = !0;
                    this.callbacksCapabilities = {};
                    this.actionHandler = {};
                    this.actionHandlerAsync = {};
                    this.nextAsync = null;
                    this.actionHandler.console_log = [
                        function (a) {
                            Object(f.d)(a);
                        },
                    ];
                    this.actionHandler.console_error = [
                        function (a) {
                            Object(f.c)(a);
                        },
                    ];
                    this.actionHandler.workerLoaded = [function () { }];
                    this.msgHandler = this.handleMessage.bind(this);
                    d.addEventListener("message", this.msgHandler);
                }
                d.prototype.on = function (b, d, a) {
                    var c = this.actionHandler;
                    c[b] && Object(f.c)('There is already an actionName called "' + b + '"');
                    c[b] = [d, a];
                };
                d.prototype.clearActionHandlers = function () {
                    this.actionHandler = {};
                    this.comObj.removeEventListener("message", this.msgHandler);
                };
                d.prototype.reset = function () {
                    this.clearActionHandlers();
                    this.comObj.reset && this.comObj.reset();
                };
                d.prototype.replace = function (b, d, a) {
                    this.actionHandler[b] = [d, a];
                };
                d.prototype.onAsync = function (b, d, a) {
                    var c = this.actionHandlerAsync;
                    c[b] && Object(f.c)('There is already an actionName called "' + b + '"');
                    c[b] = [d, a];
                };
                d.prototype.replaceAsync = function (b, d, a) {
                    var c = this.actionHandlerAsync,
                        m = this.actionHandler;
                    m[b] && delete m[b];
                    c[b] = [d, a];
                };
                d.prototype.onNextAsync = function (b) {
                    this.nextAsync = b;
                };
                d.prototype.send = function (b, d) {
                    this.postMessage({ action: b, data: d });
                };
                d.prototype.getNextId = function () {
                    return this.callbackIndex++;
                };
                d.prototype.sendWithPromise = function (b, d, a) {
                    var c = this.getNextId();
                    b = { action: b, data: d, callbackId: c, priority: a };
                    d = window.createPromiseCapability();
                    this.callbacksCapabilities[c] = d;
                    try {
                        this.postMessage(b);
                    } catch (m) {
                        d.reject(m);
                    }
                    return d.promise;
                };
                d.prototype.sendWithPromiseReturnId = function (b, d, a) {
                    var c = this.getNextId();
                    b = { action: b, data: d, callbackId: c, priority: a };
                    d = window.createPromiseCapability();
                    this.callbacksCapabilities[c] = d;
                    try {
                        this.postMessage(b);
                    } catch (m) {
                        d.reject(m);
                    }
                    return { promise: d.promise, callbackId: c };
                };
                d.prototype.sendWithPromiseWithId = function (b, d, a) {
                    d > this.callbackIndex && Object(f.c)("Can't reuse callbackId " + d + " lesser than callbackIndex " + this.callbackIndex);
                    d in this.callbacksCapabilities && Object(f.c)("Can't reuse callbackId " + d + ". There is a capability waiting to be resolved. ");
                    b = { action: b, data: a, callbackId: d };
                    a = window.createPromiseCapability();
                    this.callbacksCapabilities[d] = a;
                    try {
                        this.postMessage(b);
                    } catch (c) {
                        a.reject(c);
                    }
                    return a.promise;
                };
                d.prototype.sendError = function (b, d) {
                    if (b.message || b.errorData) {
                        b.message && b.message.message && (b.message = b.message.message);
                        var a = b.errorData;
                        b = { type: b.type ? b.type : "JavascriptError", message: b.message };
                        a &&
                            Object.keys(a).forEach(function (c) {
                                a.hasOwnProperty(c) && (b[c] = a[c]);
                            });
                    }
                    this.postMessage({ isReply: !0, callbackId: d, error: b });
                };
                d.prototype.getPromise = function (b) {
                    if (b in this.callbacksCapabilities) return this.callbacksCapabilities[b];
                    Object(f.c)("Cannot get promise for callback " + b);
                };
                d.prototype.cancelPromise = function (b) {
                    if (b in this.callbacksCapabilities) {
                        var d = this.callbacksCapabilities[b];
                        delete this.callbacksCapabilities[b];
                        d.reject({ type: "Cancelled", message: "Request has been cancelled." });
                        this.postMessage({ action: "actionCancel", data: { callbackId: b } });
                    } else Object(f.e)("Cannot cancel callback " + b);
                };
                d.prototype.postMessage = function (b) {
                    if (this.postMessageTransfers) {
                        var d = this.getTransfersArray(b);
                        this.comObj.postMessage(b, d);
                    } else this.comObj.postMessage(b);
                };
                d.prototype.getObjectTransfers = function (b, d) {
                    var a = this;
                    null !== b &&
                        "object" === typeof b &&
                        (b instanceof Uint8Array
                            ? d.push(b.buffer)
                            : b instanceof ArrayBuffer
                                ? d.push(b)
                                : Object.keys(b).forEach(function (c) {
                                    b.hasOwnProperty(c) && a.getObjectTransfers(b[c], d);
                                }));
                };
                d.prototype.getTransfersArray = function (b) {
                    var d = [];
                    this.getObjectTransfers(b, d);
                    return 0 === d.length ? void 0 : d;
                };
                d.prototype.handleMessage = function (b) {
                    var d = this,
                        a = b.data,
                        c = this.actionHandler,
                        m = this.actionHandlerAsync;
                    b = this.callbacksCapabilities;
                    if (a.isReply) (c = a.callbackId), c in b ? ((m = b[c]), delete b[c], "error" in a ? m.reject(a.error) : m.resolve(a.data)) : Object(f.a)("Cannot resolve callback " + c);
                    else if (a.action in c) {
                        var h = c[a.action];
                        a.callbackId
                            ? Promise.resolve()
                                .then(function () {
                                    return h[0].call(h[1], a.data);
                                })
                                .then(
                                    function (c) {
                                        d.postMessage({ isReply: !0, callbackId: a.callbackId, data: c });
                                    },
                                    function (c) {
                                        d.sendError(c, a.callbackId);
                                    }
                                )
                            : h[0].call(h[1], a.data);
                    } else
                        a.action in m
                            ? ((h = m[a.action]),
                                a.callbackId
                                    ? h[0].call(h[1], a).then(
                                        function (c) {
                                            d.postMessage({ isReply: !0, callbackId: a.callbackId, data: c });
                                            d.nextAsync();
                                        },
                                        function (c) {
                                            d.sendError(c, a.callbackId);
                                            d.nextAsync();
                                        }
                                    )
                                    : h[0].call(h[1], a).then(
                                        function () {
                                            d.nextAsync();
                                        },
                                        function () {
                                            d.nextAsync();
                                        }
                                    ))
                            : Object(f.c)("Unknown action from worker: " + a.action);
                };
                return d;
            })();
            l.a = d;
        },
        function (d, l, g) {
            d.exports = g(18);
        },
        function (d, l, g) {
            g.r(l);
            g(6);
            d = g(14);
            g(19);
            g(20);
            g(23);
            g(24);
            g(25);
            g(26);
            g(27);
            Object(d.a)();
        },
        function (d, l, g) {
            (function (d) {
                "undefined" === typeof d.crypto &&
                    (d.crypto = {
                        getRandomValues: function (d) {
                            for (var b = 0; b < d.length; b++) d[b] = 256 * Math.random();
                        },
                    });
            })("undefined" === typeof window ? self : window);
        },
        function (d, l, g) {
            (function (d, h) {
                function b(d) {
                    b =
                        "function" === typeof Symbol && "symbol" === typeof Symbol.iterator
                            ? function (a) {
                                return typeof a;
                            }
                            : function (a) {
                                return a && "function" === typeof Symbol && a.constructor === Symbol && a !== Symbol.prototype ? "symbol" : typeof a;
                            };
                    return b(d);
                }
                (function (d) {
                    function a() {
                        for (var a = 0; a < D.length; a++) D[a][0](D[a][1]);
                        D = [];
                        K = !1;
                    }
                    function c(e, k) {
                        D.push([e, k]);
                        K || ((K = !0), E(a, 0));
                    }
                    function m(a, e) {
                        function k(a) {
                            n(e, a);
                        }
                        function c(a) {
                            y(e, a);
                        }
                        try {
                            a(k, c);
                        } catch (F) {
                            c(F);
                        }
                    }
                    function f(a) {
                        var e = a.owner,
                            k = e.state_;
                        e = e.data_;
                        var c = a[k];
                        a = a.then;
                        if ("function" === typeof c) {
                            k = q;
                            try {
                                e = c(e);
                            } catch (F) {
                                y(a, F);
                            }
                        }
                        l(a, e) || (k === q && n(a, e), k === B && y(a, e));
                    }
                    function l(a, e) {
                        var k;
                        try {
                            if (a === e) throw new TypeError("A promises callback cannot return that same promise.");
                            if (e && ("function" === typeof e || "object" === b(e))) {
                                var c = e.then;
                                if ("function" === typeof c)
                                    return (
                                        c.call(
                                            e,
                                            function (c) {
                                                k || ((k = !0), e !== c ? n(a, c) : w(a, c));
                                            },
                                            function (e) {
                                                k || ((k = !0), y(a, e));
                                            }
                                        ),
                                        !0
                                    );
                            }
                        } catch (F) {
                            return k || y(a, F), !0;
                        }
                        return !1;
                    }
                    function n(a, e) {
                        (a !== e && l(a, e)) || w(a, e);
                    }
                    function w(a, q) {
                        a.state_ === e && ((a.state_ = k), (a.data_ = q), c(z, a));
                    }
                    function y(a, q) {
                        a.state_ === e && ((a.state_ = k), (a.data_ = q), c(A, a));
                    }
                    function u(a) {
                        var e = a.then_;
                        a.then_ = void 0;
                        for (a = 0; a < e.length; a++) f(e[a]);
                    }
                    function z(a) {
                        a.state_ = q;
                        u(a);
                    }
                    function A(a) {
                        a.state_ = B;
                        u(a);
                    }
                    function r(a) {
                        if ("function" !== typeof a) throw new TypeError("Promise constructor takes a function argument");
                        if (!(this instanceof r)) throw new TypeError("Failed to construct 'Promise': Please use the 'new' operator, this object constructor cannot be called as a function.");
                        this.then_ = [];
                        m(a, this);
                    }
                    d.createPromiseCapability = function () {
                        var a = {};
                        a.promise = new r(function (e, k) {
                            a.resolve = e;
                            a.reject = k;
                        });
                        return a;
                    };
                    var C = d.Promise,
                        t =
                            C &&
                            "resolve" in C &&
                            "reject" in C &&
                            "all" in C &&
                            "race" in C &&
                            (function () {
                                var a;
                                new C(function (e) {
                                    a = e;
                                });
                                return "function" === typeof a;
                            })();
                    "undefined" !== typeof exports && exports
                        ? ((exports.Promise = t ? C : r), (exports.Polyfill = r))
                        : "function" === typeof define && g(22)
                            ? define(function () {
                                return t ? C : r;
                            })
                            : t || (d.Promise = r);
                    var e = "pending",
                        k = "sealed",
                        q = "fulfilled",
                        B = "rejected",
                        v = function () { },
                        E = "undefined" !== typeof h ? h : setTimeout,
                        D = [],
                        K;
                    r.prototype = {
                        constructor: r,
                        state_: e,
                        then_: null,
                        data_: void 0,
                        then: function (a, e) {
                            a = { owner: this, then: new this.constructor(v), fulfilled: a, rejected: e };
                            this.state_ === q || this.state_ === B ? c(f, a) : this.then_.push(a);
                            return a.then;
                        },
                        catch: function (a) {
                            return this.then(null, a);
                        },
                    };
                    r.all = function (a) {
                        if ("[object Array]" !== Object.prototype.toString.call(a)) throw new TypeError("You must pass an array to Promise.all().");
                        return new this(function (e, k) {
                            function c(a) {
                                b++;
                                return function (k) {
                                    q[a] = k;
                                    --b || e(q);
                                };
                            }
                            for (var q = [], b = 0, d = 0, v; d < a.length; d++) (v = a[d]) && "function" === typeof v.then ? v.then(c(d), k) : (q[d] = v);
                            b || e(q);
                        });
                    };
                    r.race = function (a) {
                        if ("[object Array]" !== Object.prototype.toString.call(a)) throw new TypeError("You must pass an array to Promise.race().");
                        return new this(function (e, k) {
                            for (var c = 0, q; c < a.length; c++) (q = a[c]) && "function" === typeof q.then ? q.then(e, k) : e(q);
                        });
                    };
                    r.resolve = function (a) {
                        return a && "object" === b(a) && a.constructor === this
                            ? a
                            : new this(function (e) {
                                e(a);
                            });
                    };
                    r.reject = function (a) {
                        return new this(function (e, k) {
                            k(a);
                        });
                    };
                })("undefined" !== typeof window ? window : "undefined" !== typeof d ? d : "undefined" !== typeof self ? self : void 0);
            }.call(this, g(10), g(8).setImmediate));
        },
        function (d, l, g) {
            (function (d, h) {
                (function (b, d) {
                    function a(a) {
                        delete z[a];
                    }
                    function c(e) {
                        if (A) setTimeout(c, 0, e);
                        else {
                            var k = z[e];
                            if (k) {
                                A = !0;
                                try {
                                    var q = k.callback,
                                        b = k.args;
                                    switch (b.length) {
                                        case 0:
                                            q();
                                            break;
                                        case 1:
                                            q(b[0]);
                                            break;
                                        case 2:
                                            q(b[0], b[1]);
                                            break;
                                        case 3:
                                            q(b[0], b[1], b[2]);
                                            break;
                                        default:
                                            q.apply(d, b);
                                    }
                                } finally {
                                    a(e), (A = !1);
                                }
                            }
                        }
                    }
                    function m() {
                        C = function (a) {
                            h.nextTick(function () {
                                c(a);
                            });
                        };
                    }
                    function f() {
                        if (b.postMessage && !b.importScripts) {
                            var a = !0,
                                k = b.onmessage;
                            b.onmessage = function () {
                                a = !1;
                            };
                            b.postMessage("", "*");
                            b.onmessage = k;
                            return a;
                        }
                    }
                    function g() {
                        var a = "setImmediate$" + Math.random() + "$",
                            k = function (e) {
                                e.source === b && "string" === typeof e.data && 0 === e.data.indexOf(a) && c(+e.data.slice(a.length));
                            };
                        b.addEventListener ? b.addEventListener("message", k, !1) : b.attachEvent("onmessage", k);
                        C = function (e) {
                            b.postMessage(a + e, "*");
                        };
                    }
                    function n() {
                        var a = new MessageChannel();
                        a.port1.onmessage = function (a) {
                            c(a.data);
                        };
                        C = function (e) {
                            a.port2.postMessage(e);
                        };
                    }
                    function l() {
                        var a = r.documentElement;
                        C = function (e) {
                            var k = r.createElement("script");
                            k.onreadystatechange = function () {
                                c(e);
                                k.onreadystatechange = null;
                                a.removeChild(k);
                                k = null;
                            };
                            a.appendChild(k);
                        };
                    }
                    function y() {
                        C = function (a) {
                            setTimeout(c, 0, a);
                        };
                    }
                    if (!b.setImmediate) {
                        var u = 1,
                            z = {},
                            A = !1,
                            r = b.document,
                            C,
                            t = Object.getPrototypeOf && Object.getPrototypeOf(b);
                        t = t && t.setTimeout ? t : b;
                        "[object process]" === {}.toString.call(b.process) ? m() : f() ? g() : b.MessageChannel ? n() : r && "onreadystatechange" in r.createElement("script") ? l() : y();
                        t.setImmediate = function (a) {
                            "function" !== typeof a && (a = new Function("" + a));
                            for (var e = Array(arguments.length - 1), c = 0; c < e.length; c++) e[c] = arguments[c + 1];
                            z[u] = { callback: a, args: e };
                            C(u);
                            return u++;
                        };
                        t.clearImmediate = a;
                    }
                })("undefined" === typeof self ? ("undefined" === typeof d ? this : d) : self);
            }.call(this, g(10), g(13)));
        },
        function (d, l) {
            d.exports = {};
        },
        function (d, l, g) {
            (function (d) {
                var f = function (b, d) {
                    var a = function x(a) {
                        a = this["catch"](a);
                        return { cancel: d, promise: a, then: c.bind(a), catch: x.bind(a) };
                    },
                        c = function w(c, b) {
                            c = this.then(c, b);
                            return { cancel: d, promise: c, then: w.bind(c), catch: a.bind(c) };
                        };
                    return { cancel: d, promise: b, then: c.bind(b), catch: a.bind(b) };
                };
                d.CancellablePromise = function (b, d) {
                    var a = !1,
                        c,
                        m = new Promise(function (m, f) {
                            c = function () {
                                a || (d(), f("cancelled"));
                            };
                            new Promise(b).then(
                                function (c) {
                                    a = !0;
                                    m(c);
                                },
                                function (c) {
                                    a = !0;
                                    f(c);
                                }
                            );
                        });
                    return f(m, c);
                };
                d.CancellablePromise.all = function (b) {
                    var d = Promise.all(b);
                    return f(d, function () {
                        b.forEach(function (a) {
                            a.cancel && a.cancel();
                        });
                    });
                };
            })("undefined" === typeof self ? void 0 : self);
        },
        function (d, l, g) {
            (function (d, h) {
                var b = g(1);
                (function (f) {
                    f.Module = {
                        INITIAL_MEMORY: 50331648,
                        noExitRuntime: !0,
                        devicePixelRatio: 1,
                        cur_doc: null,
                        cachePtrSize: 0,
                        hasBufOwnership: !0,
                        loaded: !1,
                        initCb: null,
                        cachePtr: null,
                        cleanupState: null,
                        docs: {},
                        postEvent: function (a, c, b) {
                            Module.workerMessageHandler.send("event", { docId: a, type: c, data: b });
                        },
                        postPagesUpdatedEvent: function (a, c, b, d) {
                            a = { pageDimensions: Module.GetPageDimensions(a) };
                            if (b)
                                for (var f = 0; f < b.length; ++f)
                                    b[f] in a.pageDimensions ? ((a.pageDimensions[b[f]].contentChanged = !0), d && (a.pageDimensions[b[f]].annotationsUnchanged = !0)) : console.warn("Invalid Page Number ".concat(b[f]));
                            Module.postEvent(c, "pagesUpdated", a);
                            return a;
                        },
                        GetIndividualPageDimensions: function (a, c, b) {
                            a = Module.PageGetPageInfo(b);
                            a.id = Module.PageGetId(b);
                            return a;
                        },
                        GetPageDimensionsRange: function (a, c, b) {
                            for (var d = {}, f = Module.PDFDocGetPageIterator(a, c); c < b && Module.IteratorHasNext(f); ++c) {
                                var m = Module.stackSave(),
                                    h = Module.IteratorCurrent(f);
                                d[c] = this.GetIndividualPageDimensions(a, c, h);
                                Module.IteratorNext(f);
                                Module.stackRestore(m);
                            }
                            return d;
                        },
                        GetPageDimensions: function (a) {
                            try {
                                var c = Module.stackSave();
                                var b = Module.GetPageCount(a);
                                if (0 === b) throw "This document has no pages.";
                                return Module.GetPageDimensionsRange(a, 1, b + 1);
                            } finally {
                                Module.stackRestore(c);
                            }
                        },
                        loadDoc: function (a, c) {
                            "undefined" === typeof Module && this._main();
                            var b = null;
                            try {
                                var d = Module.stackSave();
                                a.customHandlerId && Module._TRN_PDFNetAddPDFTronCustomHandler(a.customHandlerId);
                                c = Module.CreateDoc(a, c);
                                var f = Module.GetDoc(c);
                                if (Module.PDFDocInitSecurityHandler(f)) return { docId: c, pageDimensions: Module.GetPageDimensions(f) };
                                b = { type: "NeedsPassword", errorData: { docId: c }, message: "This document requires a password" };
                            } catch (n) {
                                b = { type: "InvalidPDF", message: n };
                            } finally {
                                Module.stackRestore(d);
                            }
                            throw b;
                        },
                        loadCanvas: function (a, c, b, d, f, h, g, l) {
                            return new Promise(function (m, n) {
                                var u = Module.GetDoc(a),
                                    p = c + 1,
                                    w = function () {
                                        m(Module.RasterizePage(u, p, b, d, h, f, g, l));
                                    },
                                    x = Module.docs[a].chunkStorage;
                                if (x) {
                                    var e = Module.GetDownloadData(u).downloader,
                                        k = x.getRequiredChunkOffsetArrays(e, p);
                                    x.keepChunks(k.have);
                                    e = function () {
                                        var a = x.getChunks(k.missing);
                                        Module.loadPromise = a
                                            .then(function () {
                                                var a = Module.loadPromise.cancelled;
                                                Module.loadPromise = null;
                                                a || w();
                                            })
                                        ["catch"](function (a) {
                                            "cancelled" !== a ? n(a) : (Module.loadPromise = null);
                                        });
                                    };
                                    Module.loadPromise ? Module.loadPromise.then(e, e) : e();
                                } else w();
                            });
                        },
                        loadResources: function (a, c) {
                            Module.Initialize(c);
                            Object(b.b)("worker", "PDFNet initialized!");
                            Module._TRN_PDFNetSetDefaultDiskCachingEnabled(!1);
                            a = new Uint8Array(a);
                            Module.PDFNetSetResourceData(a);
                        },
                        onRuntimeInitialized: function () {
                            "undefined" === typeof Module && (("undefined" !== typeof window ? window : self).Module = {});
                            (function (a) {
                                a.PDFDocExportXFDF = function (a, b) {
                                    a = Module.GetDoc(a);
                                    var c = Module.stackSave();
                                    try {
                                        var d = b ? Module.PDFDocFDFExtract(a, b) : Module.PDFDocFDFExtract(a);
                                        var f = Module.FDFDocSaveAsXFDF(d);
                                        Module.stackRestore(c);
                                    } catch (w) {
                                        throw (Module.stackRestore(c), w);
                                    }
                                    return f;
                                };
                                a.PageArrayToPageSet = function (a) {
                                    var c = Module.stackSave();
                                    try {
                                        var b = Module.PageSetCreate();
                                        for (var d = 0; d < a.length; ++d) Module.PageSetAddPage(b, a[d]);
                                        Module.stackRestore(c);
                                    } catch (n) {
                                        throw (Module.stackRestore(c), n);
                                    }
                                    return b;
                                };
                                a.cancelCurrent = function () {
                                    var a = Module.loadPromise;
                                    return a
                                        ? (a.cancel(), (a.cancelled = !0))
                                        : (a = Module.cleanupState)
                                            ? (d(a.timeout),
                                                a.cleanupArr.forEach(function (a) {
                                                    a();
                                                }),
                                                (Module.cleanupState = null),
                                                !0)
                                            : !1;
                                };
                                a.SetWorkerRestartCallback = function (a) {
                                    Module.workerRestartCallback = a;
                                };
                                a.XFDFMerge = function (a, d, f) {
                                    if (d) {
                                        var c = [];
                                        try {
                                            Object(b.b)("worker", "Merge XFDF of length ".concat(d.length));
                                            var m = Module.GetUStringFromJSString(d, !0);
                                            c.push(function () {
                                                Module.UStringDestroy(m);
                                            });
                                            var h = Module.allocate(4, "i8", Module.ALLOC_STACK);
                                            REX(Module._TRN_FDFDocCreateFromXFDF(m, h));
                                            var g = Module.getValue(h, "i8*");
                                            c.push(function () {
                                                Module.FDFDocDestroy(g);
                                            });
                                            Module.PDFDocFDFUpdate(a, g, f);
                                        } finally {
                                            c.forEach(function (a) {
                                                a();
                                            });
                                        }
                                    }
                                };
                                a.MergeXFDF = function (a, b, d) {
                                    return new Promise(function (c, f) {
                                        var m = [];
                                        try {
                                            var h = Module.stackSave();
                                            m[m.length] = function () {
                                                Module.stackRestore(h);
                                            };
                                            var n = Module.GetDoc(a);
                                            Module.XFDFMerge(n, b, d);
                                            m.forEach(function (a) {
                                                a();
                                            });
                                            c({});
                                        } catch (z) {
                                            m.forEach(function (a) {
                                                a();
                                            }),
                                                f(z);
                                        }
                                    });
                                };
                                a.CreateBufferFile = function (a, b, d) {
                                    Module.MakeDev(a);
                                    var c = new ArrayBuffer(b);
                                    c = new Uint8Array(c);
                                    d = d ? 0 : 255;
                                    for (var f = 0; f < b; ++f) c[f] = d;
                                    Module.docs[a] = { buffer: c };
                                };
                                a.ReadBufferFile = function (a, b) {
                                    var c = Module.docs[a].buffer;
                                    b && (Module.docs[a].buffer = new Uint8Array(c.buffer.slice(0)));
                                    return c;
                                };
                                a.RemoveBufferFile = function (a) {
                                    Module.docs[a] = null;
                                };
                                a.SaveHelper = function (a, b, d) {
                                    d = "undefined" === typeof d ? 2 : d;
                                    Module.MakeDev(b);
                                    a = Module._TRN_PDFDocSave(a, Module.GetUStringFromJSString(b), d, 0);
                                    Module.docs[b].sink = null;
                                    REX(a);
                                    return Module.docs[b].buffer.buffer;
                                };
                                a.SaveDoc = function (a, b, d, h, g, l, y) {
                                    return new Promise(function (c, m) {
                                        var n = [];
                                        try {
                                            var u = Module.GetDoc(a),
                                                p = Module.stackSave();
                                            n[n.length] = function () {
                                                Module.stackRestore(p);
                                            };
                                            Module.XFDFMerge(u, b, y);
                                            var w = Module.allocate(8, "i8", Module.ALLOC_STACK),
                                                e = Module.allocate(Module.intArrayFromString('{"UseNonStandardRotation": true}'), "i8", Module.ALLOC_STACK);
                                            Module.setValue(w, e, "i8*");
                                            Module.setValue(w + 4, 0, "i32");
                                            Module._TRN_PDFDocRefreshAnnotAppearances(u, w);
                                            if (l) {
                                                w = function (a) {
                                                    a = new Uint8Array(a);
                                                    f.FS.writeFile("watermarkFile", a);
                                                    a = Module.ImageCreateFromFile(u, Module.GetUStringFromJSString("watermarkFile"));
                                                    f.FS.unlink("watermarkFile");
                                                    return a;
                                                };
                                                var k = Module.ElementBuilderCreate();
                                                n.push(function () {
                                                    Module.ElementBuilderDestroy(k);
                                                });
                                                var q = Module.ElementWriterCreate();
                                                n.push(function () {
                                                    Module.ElementWriterDestroy(q);
                                                });
                                                try {
                                                    if (!l.hasOwnProperty("default")) throw Error("Watermark dictionary has no 'default' key!");
                                                    var B = w(l["default"]);
                                                    pitr = Module.PDFDocGetPageIterator(u, 1);
                                                    for (e = -1; Module.IteratorHasNext(pitr);) {
                                                        currentPage = Module.IteratorCurrent(pitr);
                                                        Module.IteratorNext(pitr);
                                                        e++;
                                                        var v = e.toString();
                                                        try {
                                                            var E = void 0;
                                                            if (l.hasOwnProperty(v)) {
                                                                var D = l[v];
                                                                if (D) E = w(D);
                                                                else continue;
                                                            } else E = B;
                                                            var z = Module.PageGetPageInfo(currentPage),
                                                                x = Module.ElementBuilderCreateImage(k, E, 0, 0, z.width, z.height);
                                                            Module.ElementWriterBegin(q, currentPage);
                                                            Module.ElementWriterWritePlacedElement(q, x);
                                                            Module.ElementWriterEnd(q);
                                                        } catch (G) {
                                                            console.warn("Watermark for page " + v + "can not be added due to error: " + G);
                                                        }
                                                    }
                                                } catch (G) {
                                                    console.warn("Watermarks can not be added due to error: " + G);
                                                }
                                            }
                                            B = 0;
                                            if (h) {
                                                var t = Module.PDFDocGetRoot(u);
                                                (B = Module.ObjFindObj(t, "OpenAction")) && Module.ObjPut(t, "__OpenActionBackup__", B);
                                                var J = Module.ObjPutDict(t, "OpenAction");
                                                Module.ObjPutName(J, "Type", "Action");
                                                Module.ObjPutName(J, "S", "JavaScript");
                                                Module.ObjPutString(J, "JS", "this.print()");
                                            }
                                            var I = Module.SaveHelper(u, a, g);
                                            h && (B ? Module.ObjPut(t, "OpenAction", Module.ObjFindObj(t, "__OpenActionBackup__")) : Module.ObjErase(t, "OpenAction"));
                                            n.forEach(function (a) {
                                                a();
                                            });
                                            if (d) c({ fileData: I });
                                            else {
                                                var F = I.slice(0);
                                                c({ fileData: F });
                                            }
                                        } catch (G) {
                                            n.forEach(function (a) {
                                                a();
                                            }),
                                                m(G);
                                        }
                                    });
                                };
                                a.SaveDocFromFixedElements = function (a, b, d) {
                                    a = Module.PDFDocCreateFromLayoutEls(a);
                                    a = Module.CreateDoc({ type: "ptr", value: a });
                                    return Module.SaveDoc(a, b, !0, !1, d);
                                };
                                a.GetCurrentCanvasData = function (a) {
                                    var c = Module.currentRenderData;
                                    if (!c) return null;
                                    a && REX(Module._TRN_PDFRasterizerUpdateBuffer(c.rast));
                                    var d = Date.now();
                                    a = Module.ReadBufferFile("b", a);
                                    Object(b.b)("bufferTiming", "Copy took ".concat(Date.now() - d));
                                    return { pageBuf: a.buffer, pageWidth: c.out_width, pageHeight: c.out_height };
                                };
                                a.RasterizePage = function (a, d, f, g, n, l, y, u) {
                                    return new Promise(function (c, m) {
                                        Module.currentRenderData = {};
                                        var r = Module.currentRenderData;
                                        r.out_width = parseInt(f, 10);
                                        r.out_height = parseInt(g, 10);
                                        var p = [];
                                        p.push(function () {
                                            Module.currentRenderData = null;
                                        });
                                        try {
                                            var w = Module.stackSave();
                                            p[p.length] = function () {
                                                Module.stackRestore(w);
                                            };
                                            var e = Module.GetPage(a, d),
                                                k = Module.PageGetPageWidth(e),
                                                q = Module.PageGetPageHeight(e);
                                            r.stride = 4 * r.out_width;
                                            r.buf_size = r.out_width * r.out_height * 4;
                                            Object(b.b)("Memory", "Created rasterizer");
                                            r.rast = Module.PDFRasterizerCreate();
                                            p.push(function () {
                                                Object(b.b)("Memory", "Destroyed rasterizer");
                                                Module._TRN_PDFRasterizerDestroy(r.rast);
                                            });
                                            if (y) {
                                                var B = Module.EMSCreateUpdatedLayersContext(a, y);
                                                0 !== B &&
                                                    (REX(Module._TRN_PDFRasterizerSetOCGContext(r.rast, B)),
                                                        p.push(function () {
                                                            Module._TRN_OCGContextDestroy(B);
                                                        }));
                                            }
                                            u.hasOwnProperty("renderAnnots") ? REX(Module._TRN_PDFRasterizerSetDrawAnnotations(r.rast, u.renderAnnots ? 1 : 0)) : REX(Module._TRN_PDFRasterizerSetDrawAnnotations(r.rast, 0));
                                            u.hasOwnProperty("highlightFields") && REX(Module._TRN_PDFRasterizerSetHighlightFields(r.rast, u.highlightFields));
                                            u.hasOwnProperty("antiAliasing") && REX(Module._TRN_PDFRasterizerSetAntiAliasing(r.rast, u.antiAliasing));
                                            u.hasOwnProperty("pathHinting") && REX(Module._TRN_PDFRasterizerSetPathHinting(r.rast, u.pathHinting));
                                            if (u.hasOwnProperty("thinLinePixelGridFit")) {
                                                var v = !0;
                                                u.hasOwnProperty("thinLineStrokeAdjust") && (v = u.thinLineStrokeAdjust);
                                                REX(Module._TRN_PDFRasterizerSetThinLineAdjustment(r.rast, u.thinLinePixelGridFit, v));
                                            } else u.hasOwnProperty("thinLineStrokeAdjust") && REX(Module._TRN_PDFRasterizerSetThinLineAdjustment(r.rast, !1, u.thinLineStrokeAdjust));
                                            u.hasOwnProperty("imageSmoothing")
                                                ? ((v = !1), u.hasOwnProperty("hqImageResampling") && (v = u.hqImageResampling), REX(Module._TRN_PDFRasterizerSetImageSmoothing(r.rast, u.imageSmoothing, v)))
                                                : u.hasOwnProperty("hqImageResampling") && REX(Module._TRN_PDFRasterizerSetImageSmoothing(r.rast, !0, u.hqImageResampling));
                                            u.hasOwnProperty("caching") && REX(Module._TRN_PDFRasterizerSetCaching(r.rast, u.caching));
                                            u.hasOwnProperty("expGamma") && REX(Module._TRN_PDFRasterizerSetGamma(r.rast, u.expGamma));
                                            u.hasOwnProperty("isPrinting") && REX(Module._TRN_PDFRasterizerSetPrintMode(r.rast, u.isPrinting));
                                            u.hasOwnProperty("colorPostProcessMode") && REX(Module._TRN_PDFRasterizerSetColorPostProcessMode(r.rast, u.colorPostProcessMode));
                                            var E = Module.PageGetRotation(e);
                                            v = 1 === l || 3 === l;
                                            E = (1 === E || 3 === E) !== v;
                                            var D = Module.allocate(48, "i8", Module.ALLOC_STACK);
                                            if (n) {
                                                n.x1 = n[0];
                                                n.y1 = n[1];
                                                n.x2 = n[2];
                                                n.y2 = n[3];
                                                var A = Module.PageGetDefaultMatrix(e, 0),
                                                    z = Module.Matrix2DInverse(A);
                                                n = Module.Matrix2DMultBBox(z, n);
                                                if (n.x2 < n.x1) {
                                                    var x = n.x1;
                                                    n.x1 = n.x2;
                                                    n.x2 = x;
                                                }
                                                n.y2 < n.y1 && ((x = n.y1), (n.y1 = n.y2), (n.y2 = x));
                                                var t = r.out_width / (E ? n.y2 - n.y1 : n.x2 - n.x1);
                                                var I = Module.GetDefaultMatrixBox(e, n, l);
                                            } else (I = Module.PageGetDefaultMatrix(e, l)), (t = r.out_width / (v ? q : k));
                                            Module.Matrix2DSet(D, t, 0, 0, t, 0, 0);
                                            Module.Matrix2DConcat(D, I);
                                            Object(b.b)("Memory", "Allocated buffer of ".concat(r.buf_size));
                                            p.push(function () {
                                                Object(b.b)("Memory", "Deallocated buffer of ".concat(r.buf_size));
                                                Module._free(r.bufPtr);
                                            });
                                            var F = Module.allocate(4, "i8", Module.ALLOC_STACK),
                                                G = Module.allocate(4, "i8", Module.ALLOC_STACK);
                                            Module.CreateBufferFile("b", r.buf_size, u.pageTransparent);
                                            p.push(function () {
                                                Module.RemoveBufferFile("b");
                                            });
                                            var M = u.overprintMode;
                                            if (10 === M) {
                                                REX(Module._TRN_PDFRasterizerSetOverprint(r.rast, 1));
                                                var P = Module.PDFRasterizerRasterizeSeparations(r.rast, e, r.out_width, r.out_height, D, 0, 0);
                                                c({ pageBuf: P, pageWidth: r.out_width, pageHeight: r.out_height });
                                            } else {
                                                REX(Module._TRN_PDFRasterizerSetOverprint(r.rast, M));
                                                REX(Module._TRN_PDFRasterizerGetChunkRendererPath(r.rast, e, Module.GetUStringFromJSString("b"), r.out_width, r.out_height, !0, D, 0, 0, 0, F));
                                                var L = Module.getValue(F, "i8*");
                                                p.splice(1, 0, function () {
                                                    REX(Module._TRN_ChunkRendererDestroy(L));
                                                });
                                            }
                                            var O = new Date().getTime(),
                                                S = h(function Q() {
                                                    try {
                                                        for (var a = 0, e = new Date().getTime(), k = !1; 200 > a;) {
                                                            REX(Module._TRN_ChunkRendererRenderNext(L, G));
                                                            if (!Module.getValue(G, "i8*")) {
                                                                k = !0;
                                                                break;
                                                            }
                                                            a = new Date().getTime() - e;
                                                        }
                                                        if (k) {
                                                            var d = Module.GetCurrentCanvasData(!1);
                                                            Object(b.b)("worker", "Total Page Time ".concat(new Date().getTime() - O));
                                                            p.forEach(function (a) {
                                                                a();
                                                            });
                                                            c(d);
                                                        } else Module.cleanupState.timeout = h(Q);
                                                    } catch (R) {
                                                        p.forEach(function (a) {
                                                            a();
                                                        }),
                                                            m(R);
                                                    }
                                                });
                                            Module.cleanupState = { cleanupArr: p, timeout: S };
                                            p.push(function () {
                                                Module.cleanupState = null;
                                            });
                                        } catch (T) {
                                            p.forEach(function (a) {
                                                a();
                                            }),
                                                m(T);
                                        }
                                    });
                                };
                                a.UpdatePassword = function (a, b) {
                                    try {
                                        var c = Module.stackSave();
                                        var d = Module.GetDoc(a);
                                        return Module.PDFDocInitStdSecurityHandler(d, b)
                                            ? (d in downloadDataMap && REX(Module._TRN_PDFDocDownloaderInitialize(d, downloadDataMap[d].downloader)), { success: !0, pageDimensions: Module.GetPageDimensions(d) })
                                            : { success: !1 };
                                    } finally {
                                        Module.stackRestore(c);
                                    }
                                };
                                a.InsertBlankPages = function (a, b, d, f) {
                                    return new Promise(function (c, m) {
                                        var h = [],
                                            g = Module.GetDoc(a);
                                        try {
                                            var n = Module.stackSave();
                                            h[h.length] = function () {
                                                Module.stackRestore(n);
                                            };
                                            for (var l = b.length - 1; 0 <= l; --l) {
                                                var r = Module.PDFDocGetPageIterator(g, b[l]),
                                                    p = Module.PDFDocPageCreate(g, d, f);
                                                Module.PDFDocPageInsert(g, r, p);
                                            }
                                            var w = Module.postPagesUpdatedEvent(g, a);
                                            h.forEach(function (a) {
                                                a();
                                            });
                                            c(w);
                                        } catch (e) {
                                            h.forEach(function (a) {
                                                a();
                                            }),
                                                m(e);
                                        }
                                    });
                                };
                                a.InsertPages = function (a, b, d, f, h) {
                                    return new Promise(function (c, m) {
                                        var g = [],
                                            n = Module.GetDoc(a);
                                        try {
                                            var l = Module.stackSave();
                                            g[g.length] = function () {
                                                Module.stackRestore(l);
                                            };
                                            if (b instanceof ArrayBuffer) {
                                                var r = Module.CreateDoc(b);
                                                var p = Module.GetDoc(r);
                                                g[g.length] = function () {
                                                    Module.DeleteDoc(r);
                                                };
                                            } else p = Module.GetDoc(b);
                                            for (var w = d.length, e = Module.PageSetCreate(), k = 0; k < w; ++k) Module.PageSetAddPage(e, d[k]);
                                            Module.PDFDocInsertPages(n, f, p, e, h);
                                            var q = Module.postPagesUpdatedEvent(n, a);
                                            g.forEach(function (a) {
                                                a();
                                            });
                                            c(q);
                                        } catch (B) {
                                            g.forEach(function (a) {
                                                a();
                                            }),
                                                m(B);
                                        }
                                    });
                                };
                                a.MovePages = function (a, b, d) {
                                    return new Promise(function (c, f) {
                                        var m = [],
                                            h = Module.GetDoc(a);
                                        try {
                                            var g = Module.stackSave();
                                            m[m.length] = function () {
                                                Module.stackRestore(g);
                                            };
                                            for (var n = b.length, l = Module.PageSetCreate(), r = 0; r < n; ++r) Module.PageSetAddPage(l, b[r]);
                                            Module.PDFDocMovePages(h, d, l);
                                            var p = Module.postPagesUpdatedEvent(h, a);
                                            m.forEach(function (a) {
                                                a();
                                            });
                                            c(p);
                                        } catch (H) {
                                            m.forEach(function (a) {
                                                a();
                                            }),
                                                f(H);
                                        }
                                    });
                                };
                                a.RemovePages = function (a, b) {
                                    return new Promise(function (c, d) {
                                        var f = Module.GetDoc(a),
                                            m = [];
                                        try {
                                            var h = Module.stackSave();
                                            m[m.length] = function () {
                                                Module.stackRestore(h);
                                            };
                                            for (var g = b.length - 1; 0 <= g; --g) {
                                                var l = Module.PDFDocGetPageIterator(f, b[g]);
                                                Module.IteratorHasNext(l) && Module.PDFDocPageRemove(f, l);
                                            }
                                            var p = Module.postPagesUpdatedEvent(f, a);
                                            m.forEach(function (a) {
                                                a();
                                            });
                                            c(p);
                                        } catch (r) {
                                            m.forEach(function (a) {
                                                a();
                                            }),
                                                d(r);
                                        }
                                    });
                                };
                                a.RotatePages = function (a, b, d) {
                                    return new Promise(function (c, f) {
                                        var m = Module.GetDoc(a),
                                            h = [];
                                        try {
                                            var g = Module.stackSave();
                                            h[h.length] = function () {
                                                Module.stackRestore(g);
                                            };
                                            var l = b.length,
                                                n = 0,
                                                r = Module.PDFDocGetPageIterator(m, b[0]),
                                                p = [];
                                            h.push(function () {
                                                Module._TRN_IteratorDestroy(r);
                                            });
                                            for (var t = b[0]; Module.IteratorHasNext(r) && n < b[l - 1]; ++t) {
                                                if (t === b[n]) {
                                                    var e = Module.IteratorCurrent(r),
                                                        k = (Module.PageGetRotation(e) + d) % 4;
                                                    Module.PageSetRotation(e, k);
                                                    p.push(t);
                                                    n++;
                                                }
                                                Module.IteratorNext(r);
                                            }
                                            var q = Module.postPagesUpdatedEvent(m, a, p, !0);
                                            h.forEach(function (a) {
                                                a();
                                            });
                                            c(q);
                                        } catch (B) {
                                            h.forEach(function (a) {
                                                a();
                                            }),
                                                f(B);
                                        }
                                    });
                                };
                                a.ExtractPages = function (a, b, d, f, h) {
                                    return new Promise(function (c, m) {
                                        var g = [];
                                        try {
                                            var l = Module.GetDoc(a),
                                                n = Module.stackSave();
                                            g[g.length] = function () {
                                                Module.stackRestore(n);
                                            };
                                            var r = function (a) {
                                                g.forEach(function (a) {
                                                    a();
                                                });
                                                m(a);
                                            };
                                            Module.XFDFMerge(l, d, h);
                                            var p = Module.CreateEmptyDoc();
                                            g[g.length] = function () {
                                                Module.DeleteDoc(p);
                                            };
                                            var w = Module.InsertPages(p, a, b, 1, !0)
                                                .then(function () {
                                                    return Module.SaveDoc(p, void 0, !0, !1, void 0, f);
                                                })
                                                .then(function (a) {
                                                    g.forEach(function (a) {
                                                        a();
                                                    });
                                                    return a;
                                                })
                                            ["catch"](r);
                                            c(w);
                                        } catch (e) {
                                            r(e);
                                        }
                                    });
                                };
                                a.CropPages = function (a, b, d, f, h, g) {
                                    return new Promise(function (c, m) {
                                        var l = Module.GetDoc(a),
                                            n = [];
                                        try {
                                            var u = Module.stackSave();
                                            n[n.length] = function () {
                                                Module.stackRestore(u);
                                            };
                                            var p = b.length,
                                                w = 0,
                                                e = Module.PDFDocGetPageIterator(l, b[0]);
                                            n.push(function () {
                                                Module._TRN_IteratorDestroy(e);
                                            });
                                            for (var k = [], q = b[0]; Module.IteratorHasNext(e) && w < b[p - 1]; ++q) {
                                                if (q === b[w]) {
                                                    var B = Module.IteratorCurrent(e),
                                                        v = Module.allocate(8, "i8", Module.ALLOC_STACK);
                                                    REX(Module._TRN_PageGetCropBox(B, v));
                                                    var E = Module.PageGetRotation(B),
                                                        D = Module.getValue(v, "double"),
                                                        t = Module.getValue(v + 8, "double"),
                                                        x = Module.getValue(v + 16, "double"),
                                                        y = Module.getValue(v + 24, "double");
                                                    0 === E % 4
                                                        ? (Module.setValue(v, D + h, "double"), Module.setValue(v + 8, t + f, "double"), Module.setValue(v + 16, x - g, "double"), Module.setValue(v + 24, y - d, "double"))
                                                        : 1 === E % 4
                                                            ? (Module.setValue(v, D + d, "double"), Module.setValue(v + 8, t + h, "double"), Module.setValue(v + 16, x - f, "double"), Module.setValue(v + 24, y - g, "double"))
                                                            : 2 === E % 4
                                                                ? (Module.setValue(v, D + g, "double"), Module.setValue(v + 8, t + d, "double"), Module.setValue(v + 16, x - h, "double"), Module.setValue(v + 24, y - f, "double"))
                                                                : 3 === E % 4 && (Module.setValue(v, D + f, "double"), Module.setValue(v + 8, t + g, "double"), Module.setValue(v + 16, x - d, "double"), Module.setValue(v + 24, y - h, "double"));
                                                    Module.setValue(v + 32, 0, "double");
                                                    REX(Module._TRN_PageSetBox(B, 0, v));
                                                    REX(Module._TRN_PageSetBox(B, 1, v));
                                                    k.push(q);
                                                    w++;
                                                }
                                                Module.IteratorNext(e);
                                            }
                                            var J = Module.postPagesUpdatedEvent(l, a, k, !0);
                                            n.forEach(function (a) {
                                                a();
                                            });
                                            c(J);
                                        } catch (I) {
                                            n.forEach(function (a) {
                                                a();
                                            }),
                                                m(I);
                                        }
                                    });
                                };
                            })("undefined" === typeof self ? this.Module : self.Module);
                            this.loaded = !0;
                            Module.initCb && Module.initCb();
                        },
                    };
                })(self);
            }.call(this, g(8).clearImmediate, g(8).setImmediate));
        },
        function (d, l, g) {
            (function (d) {
                function f(a) {
                    f =
                        "function" === typeof Symbol && "symbol" === typeof Symbol.iterator
                            ? function (a) {
                                return typeof a;
                            }
                            : function (a) {
                                return a && "function" === typeof Symbol && a.constructor === Symbol && a !== Symbol.prototype ? "symbol" : typeof a;
                            };
                    return f(a);
                }
                var b = g(1),
                    l = "undefined" !== typeof window ? window : self;
                l.global = l;
                (function (a) {
                    a.currentFileString = "/current";
                    var c = 0,
                        m = 0,
                        h = {},
                        g = null;
                    Module.chunkMax = 200;
                    var n = function (a, k, b, c, d) {
                        var e = new XMLHttpRequest();
                        return CancellablePromise(
                            function (q, f) {
                                e.open("GET", a, !0);
                                e.responseType = "arraybuffer";
                                e.onerror = function () {
                                    f("Network error occurred");
                                };
                                e.onload = function () {
                                    if (206 === this.status && e.response.byteLength === b) {
                                        var a = new Int8Array(e.response);
                                        q(a);
                                    } else f("Download Failed");
                                };
                                var v = ["bytes=", k, "-", k + b - 1].join("");
                                e.setRequestHeader("Range", v);
                                d && (e.withCredentials = d);
                                c &&
                                    Object.keys(c).forEach(function (a) {
                                        e.setRequestHeader(a, c[a]);
                                    });
                                e.send();
                            },
                            function () {
                                e.abort();
                            }
                        );
                    },
                        w = function (a) {
                            this.file = a;
                            this.filePosition = 0;
                            this.fileLength = a.size;
                            this.chunkData = null;
                            this.chunkPosition = 0;
                            this.chunkSize = 2097152;
                            this.reader = new FileReaderSync();
                        };
                    w.prototype = {
                        read: function (a, k, b) {
                            b = this.filePosition + b <= this.fileLength ? b : this.fileLength - this.filePosition;
                            a = a.subarray(k, k + b);
                            k = b;
                            if (0 < b && this.chunkData && this.chunkPosition < this.chunkData.length) {
                                var e = this.chunkPosition + b;
                                e <= this.chunkData.length
                                    ? (a.set(this.chunkData.subarray(this.chunkPosition, e)), (this.chunkPosition = e), (this.filePosition += b), (b = 0))
                                    : (a.set(this.chunkData.subarray(this.chunkPosition)), (this.filePosition += this.chunkData.length - this.chunkPosition), (b = e - this.chunkData.length));
                            }
                            0 < b &&
                                (this.fileLength <= this.chunkSize
                                    ? ((this.chunkData = new Int8Array(this.reader.readAsArrayBuffer(this.file))),
                                        (this.chunkPosition = this.filePosition),
                                        a.set(this.chunkData.subarray(this.chunkPosition, this.chunkPosition + b)),
                                        (this.chunkPosition += b))
                                    : b < this.chunkSize
                                        ? ((this.chunkPosition = 0),
                                            (e = this.filePosition),
                                            e + this.chunkSize > this.fileLength && ((e = this.fileLength - this.chunkSize), (this.chunkPosition = this.filePosition - e)),
                                            (this.chunkData = new Int8Array(this.reader.readAsArrayBuffer(this.file.slice(e, e + this.chunkSize)))),
                                            a.set(this.chunkData.subarray(this.chunkPosition, this.chunkPosition + b), k - b),
                                            (this.chunkPosition += b))
                                        : ((e = new Int8Array(this.reader.readAsArrayBuffer(this.file.slice(this.filePosition, this.filePosition + b)))), a.set(e), (this.chunkPosition = 0), (this.chunkData = null)),
                                    (this.filePosition += b));
                            return k;
                        },
                        seek: function (a) {
                            this.chunkData &&
                                ((this.chunkPosition += a - this.filePosition),
                                    this.fileLength > this.chunkSize && (this.chunkPosition > this.chunkData.length || 0 > this.chunkPosition) && ((this.chunkPosition = 0), (this.chunkData = null)));
                            this.filePosition = a;
                        },
                        close: function () {
                            this.reader = this.file = null;
                        },
                        getPos: function () {
                            return this.filePosition;
                        },
                        getTotalSize: function () {
                            return this.fileLength;
                        },
                    };
                    var t = function (a) {
                        this.data = a;
                        this.position = 0;
                        this.length = this.data.length;
                    };
                    t.prototype = {
                        read: function (a, k, b) {
                            b = this.position + b <= this.length ? b : this.length - this.position;
                            a = a.subarray(k, k + b);
                            k = this.data.subarray(this.position, this.position + b);
                            a.set(k);
                            this.position += b;
                            return b;
                        },
                        write: function (a, k, b) {
                            b = this.position + b <= this.length ? b : this.length - this.position;
                            a = a.subarray(k, k + b);
                            this.data.subarray(this.position, this.position + b).set(a);
                            this.position += b;
                            return b;
                        },
                        seek: function (a) {
                            this.position = a;
                        },
                        close: function () {
                            this.data = null;
                        },
                        getPos: function () {
                            return this.position;
                        },
                        getTotalSize: function () {
                            return this.length;
                        },
                    };
                    var u = function (a, k, b, c, d) {
                        "object" === f(a)
                            ? ((this.lruList = a.lruList), (this.chunkMap = a.chunkMap), (this.length = a.length), (this.url = a.url), (this.customHeaders = a.customHeaders), (this.withCredentials = a.withCredentials))
                            : ((this.lruList = []), (this.chunkMap = {}), (this.chunkMap[k] = d), (this.length = k), (this.url = a), (this.customHeaders = b), (this.withCredentials = c));
                    };
                    u.prototype = {
                        lruUpdate: function (a) {
                            var e = this.lruList.lastIndexOf(a);
                            0 <= e && this.lruList.splice(e, 1);
                            this.lruList.push(a);
                        },
                        getChunk: function (a) {
                            var e = this;
                            if (this.chunkMap[a]) this.lruUpdate(a);
                            else {
                                var b = Math.min(a + 1048576, this.length) - 1,
                                    c = new XMLHttpRequest();
                                c.open("GET", this.url, !1);
                                c.responseType = "arraybuffer";
                                c.setRequestHeader("Range", ["bytes=", a, "-", b].join(""));
                                this.withCredentials && (c.withCredentials = this.withCredentials);
                                this.customHeaders &&
                                    Object.keys(this.customHeaders).forEach(function (a) {
                                        c.setRequestHeader(a, e.customHeaders[a]);
                                    });
                                c.send();
                                if (200 === c.status || 206 === c.status) this.writeChunk(new Int8Array(c.response), a);
                                else throw Error("Failed to load data from");
                            }
                            return this.chunkMap[a];
                        },
                        hadChunk: function (a) {
                            return a in this.chunkMap;
                        },
                        hasChunk: function (a) {
                            return this.chunkMap[a];
                        },
                        getCacheData: function () {
                            return this.chunkMap[this.length];
                        },
                        getRequiredChunkOffsetArrays: function (a, k) {
                            var e = { have: [], downloading: [], missing: [] };
                            try {
                                var b = Module.stackSave();
                                var c = Module.allocate(4, "i8", Module.ALLOC_STACK);
                                REX(Module._TRN_DownloaderGetRequiredChunksSize(a, k, c));
                                var d = Module.getValue(c, "i8*");
                                if (d) {
                                    var f = Module._malloc(4 * d);
                                    REX(Module._TRN_DownloaderGetRequiredChunks(a, k, f, d));
                                    for (a = 0; a < d; ++a) {
                                        var g = Module.getValue(f + 4 * a, "i8*");
                                        this.hasChunk(g) ? e.have.push(g) : this.hadChunk(g) ? e.missing.push(g) : e.downloading.push(g);
                                    }
                                }
                            } finally {
                                f && Module._free(f), Module.stackRestore(b);
                            }
                            return e;
                        },
                        keepVisibleChunks: function (a, k) {
                            for (var e = k.length, b = Module.chunkMax / 2, c = 0, d = 0; d < e; ++d) {
                                var f = this.getRequiredChunkOffsetArrays(a, k[d]),
                                    g = f.have,
                                    h = g.length;
                                c += h;
                                if (c > b) {
                                    this.keepChunks(g.slice(0, h - c + b));
                                    break;
                                }
                                this.keepChunks(f.have);
                            }
                        },
                        getChunkAsync: function (a) {
                            var e = this,
                                b = a + 1048576,
                                c = 1048576;
                            b > this.length && (c -= b - this.length);
                            return n(this.url, a, c, this.customHeaders, this.withCredentials).then(function (k) {
                                e.writeChunk(k, a);
                            });
                        },
                        getChunks: function (a) {
                            for (var e = a.length, b = Array(e), c = 0; c < e; ++c) b[c] = this.getChunkAsync(a[c]);
                            return CancellablePromise.all(b);
                        },
                        keepChunks: function (a) {
                            for (var e = a.length, b = 0; b < e; ++b) this.lruUpdate(a[b]);
                        },
                        writeChunk: function (a, k, b) {
                            b = b || 0;
                            var e = this.chunkMap[k],
                                c = a.length,
                                d = this.lruList.length >= Module.chunkMax && !e;
                            1048576 !== c || a.buffer.byteLength !== c
                                ? (d ? ((e = this.lruList.shift()), (d = this.chunkMap[e]), 1048576 > d.length && (d = new Int8Array(1048576)), (this.chunkMap[e] = null)) : (d = e ? this.chunkMap[k] : new Int8Array(1048576)),
                                    d.subarray(b, b + c).set(a),
                                    (a = d))
                                : d && ((e = this.lruList.shift()), (this.chunkMap[e] = null));
                            this.lruUpdate(k);
                            this.chunkMap[k] = a;
                        },
                    };
                    var z = function (a) {
                        this.chunkStorage = a;
                        this.position = 0;
                        this.length = this.chunkStorage.length;
                    };
                    z.prototype = {
                        read: function (a, k, b) {
                            var e = this.position + b <= this.length,
                                c = e ? b : this.length - this.position;
                            if (this.position < this.length) {
                                var d;
                                for (d = 0; d < c;) {
                                    var q = this.position % 1048576;
                                    var f = this.position - q;
                                    var g = c - d;
                                    if (this.chunkStorage.hadChunk(f)) {
                                        f = this.chunkStorage.getChunk(f);
                                        var h = a.subarray(k + d, k + d + g);
                                        q = f.subarray(q, q + g);
                                        h.set(q);
                                        q = q.length;
                                        d += q;
                                        this.position += q;
                                    } else for (this.position += g; d < c; ++d) h[d] = 0;
                                }
                            }
                            if (!e) {
                                h = k + c;
                                if ((b -= c)) (k = this.chunkStorage.getCacheData()), b > k.length && (b = k.length), (e = this.position - this.length), (h = a.subarray(h, h + b)), (q = k.subarray(e, e + b)), h.set(q);
                                this.position += b;
                                return c + b;
                            }
                            return c;
                        },
                        write: function (a, k, b) {
                            var e = this.position + b <= this.length,
                                c = this.position + b <= this.length ? b : this.length - this.position,
                                d = a.subarray(k, k + c),
                                f = this.position % 1048576;
                            this.chunkStorage.writeChunk(d, this.position - f, f);
                            this.position += c;
                            if (!e) {
                                d = k + c;
                                if ((b -= c)) (k = this.chunkStorage.getCacheData()), b > k.length && (b = k.length), (e = this.position - this.length), (d = a.subarray(d, d + b)), k.subarray(e, e + b).set(d);
                                this.position += b;
                                return c + b;
                            }
                            return c;
                        },
                        seek: function (a) {
                            this.position = a;
                        },
                        close: function () {
                            this.chunkStorage = null;
                        },
                        getPos: function () {
                            return this.position;
                        },
                        getTotalSize: function () {
                            return this.length;
                        },
                    };
                    var A = function (a) {
                        this.docId = a;
                        this.length = 0;
                        this.data = new Int8Array(8192);
                        this.position = 0;
                    };
                    A.prototype = {
                        seek: function (a) {
                            this.position = a;
                        },
                        close: function () {
                            var a = new Int8Array(this.data.buffer.slice(0, this.length));
                            Module.ChangeDocBackend(this.docId, { ptr: Module.GetDoc(this.docId), buffer: a });
                            this.data = null;
                        },
                        getPos: function () {
                            return this.position;
                        },
                        getTotalSize: function () {
                            return this.length;
                        },
                        read: function (a, k, b) {
                            var e = this.data.length;
                            b = b + k < e ? b : e - k;
                            a = a.subarray(k, k + b);
                            k = this.data.subarray(this.position, this.position + b);
                            a.set(k);
                            this.position += b;
                            return b;
                        },
                        write: function (a, k, b) {
                            for (var e = this.position + b, c = this.data.length; e > c;) {
                                c = Math.max(c * (16777216 < c ? 1.5 : 2), e);
                                var d = new Int8Array(c);
                                d.set(this.data.subarray(0, this.length), 0);
                                this.data = d;
                            }
                            a = a.subarray(k, k + b);
                            this.data.set(a, this.position);
                            this.position += b;
                            this.position > this.length && (this.length = this.position);
                            return b;
                        },
                    };
                    var r = {
                        IsSink: function (a) {
                            return 66 === (a.flags & 255);
                        },
                        open: function (a) {
                            var e = a.path.slice(1);
                            this.IsSink(a)
                                ? ((a.provider = new A(e)), (Module.docs[e].sink = a.provider))
                                : (a.provider = Module.docs[e].sink
                                    ? new t(Module.docs[e].sink.data)
                                    : Module.docs[e].chunkStorage
                                        ? new z(Module.docs[e].chunkStorage)
                                        : Module.docs[e].buffer
                                            ? new t(Module.docs[e].buffer)
                                            : new w(Module.docs[e].file));
                        },
                        close: function (a) {
                            a.provider.close();
                        },
                        read: function (a, k, b, c, d) {
                            return a.provider.read(k, b, c);
                        },
                        llseek: function (a, k, b) {
                            a = a.provider;
                            1 === b ? (k += a.getPos()) : 2 === b && (k = a.getTotalSize() + k);
                            if (0 > k) throw new FS.ErrnoError(l.ERRNO_CODES.EINVAL);
                            a.seek(k);
                            return k;
                        },
                        write: function (a, k, b, c, d) {
                            return c ? a.provider.write(k, b, c) : 0;
                        },
                    };
                    l.THROW = function (a) {
                        throw { type: "InvalidPDF", message: a };
                    };
                    var C = function (e) {
                        return "Exception: \n\t Message: "
                            .concat(a.GetJSStringFromCString(Module._TRN_GetMessage(e)), "\n\t Filename: ")
                            .concat(a.GetJSStringFromCString(Module._TRN_GetFileName(e)), "\n\t Function: ")
                            .concat(a.GetJSStringFromCString(Module._TRN_GetFunction(e)), "\n\t Linenumber: ")
                            .concat(a.GetJSStringFromCString(Module._TRN_GetLineNum(e)));
                    };
                    a.GetErrToString = C;
                    l.REX = function (a) {
                        a && THROW(C(a));
                    };
                    a.Initialize = function (a) {
                        var e = Module.stackSave();
                        a = a ? Module.allocate(Module.intArrayFromString(a), "i8", Module.ALLOC_STACK) : 0;
                        REX(Module._TRN_PDFNetInitialize(a));
                        Module.stackRestore(e);
                    };
                    a.GetDoc = function (a) {
                        if (a in Module.docs) return Module.docs[a].ptr;
                        throw { type: "InvalidDocReference", message: "Unable to access Document id=".concat(a, ". The document appears to be invalid or was deleted.") };
                    };
                    a.clearDocBackend = function () {
                        null !== Module.cachePtr ? (Module.hasBufOwnership && Module._free(Module.cachePtr), (Module.cachePtr = null)) : Module.docs[a.currentFileString] && delete Module.docs[a.currentFileString];
                    };
                    a.MakeDev = function (a) {
                        if (!h[a]) {
                            var e = FS.makedev(3, 5);
                            FS.registerDevice(e, r);
                            FS.mkdev(a, 511, e);
                            h[a] = !0;
                        }
                    };
                    a.CreateDocFileBackend = function (a, k) {
                        Module.MakeDev(k);
                        var e = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        Module.docs[k] = { file: a };
                        a = Module.allocate(Module.intArrayFromString(k), "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_PDFDocCreateFromFilePath(a, e));
                        e = Module.getValue(e, "i8*");
                        Module.docs[k].ptr = e;
                    };
                    a.InsertImageIntoDoc = function (a, k, b) {
                        var e = [];
                        try {
                            var c = Module.ElementBuilderCreate();
                            e.push(function () {
                                Module.ElementBuilderDestroy(c);
                            });
                            var d = Module.ElementWriterCreate();
                            e.push(function () {
                                Module.ElementWriterDestroy(d);
                            });
                            if (b) {
                                var f = b.width;
                                var q = b.height;
                            } else (f = Module.ImageGetImageWidth(k)), (q = Module.ImageGetImageHeight(k)), (b = f / q), b > 612 / 792 ? ((f = 612), (q = parseInt(f / b, 10))) : ((q = 792), (f = parseInt(q * b, 10)));
                            var g = Module.ElementBuilderCreateImage(c, k, 0, 0, f, q),
                                h = Module.PDFDocPageCreate(a, f, q);
                            Module.ElementWriterBegin(d, h);
                            Module.ElementWriterWritePlacedElement(d, g);
                            Module.ElementWriterEnd(d);
                            Module.PDFDocPagePushBack(a, h);
                        } finally {
                            e.forEach(function (a) {
                                a();
                            });
                        }
                    };
                    var H = function (a, b, c) {
                        "object" === f(a)
                            ? ((this.m_pages = a.m_pages),
                                (this.m_has_named_dests = a.m_has_named_dests),
                                (this.m_finished_download = a.m_finished_download),
                                (this.m_has_outline = a.m_has_outline),
                                (this.m_current_page = a.m_current_page),
                                (this.m_id = a.m_id),
                                (this.size = a.size),
                                (this.timeout = a.timeout),
                                (this.eventPageArray = a.eventPageArray),
                                (this.requirePageCallbacks = a.requirePageCallbacks))
                            : ((this.m_pages = []),
                                (this.m_has_outline = this.m_finished_download = this.m_has_named_dests = !1),
                                (this.m_current_page = 1),
                                (this.m_id = c),
                                (this.size = a),
                                (this.timeout = null),
                                (this.eventPageArray = []),
                                (this.requirePageCallbacks = {}));
                        this.downloadUserData = Module.createDownloadUserData(b, c);
                    };
                    H.prototype = {
                        getJSUrl: function () {
                            return Module.extractDownloadUserData(this.downloadUserData).url;
                        },
                        getDocId: function () {
                            return Module.extractDownloadUserData(this.downloadUserData).docId;
                        },
                        destroyUserData: function () {
                            this.m_id in Module.withCredentials && delete Module.withCredentials[this.m_id];
                            this.m_id in Module.customHeadersMap && delete Module.customHeadersMap[this.m_id];
                            Module.destroyDownloadUserData(this.downloadUserData);
                        },
                    };
                    a.createDownloadUserData = function (a, b) {
                        a = Module.allocate(Module.intArrayFromString(a), "i8", Module.ALLOC_NORMAL);
                        var e = Module.allocate(8, "i8", Module.ALLOC_NORMAL);
                        Module.setValue(e, a, "i8*");
                        Module.setValue(e + 4, parseInt(b, 10), "i32");
                        return (this.downloadUserData = e);
                    };
                    a.extractDownloadUserData = function (e) {
                        var b = Module.getValue(e, "i8*");
                        b = a.GetJSStringFromCString(b);
                        e = Module.getValue(e + 4, "i32").toString();
                        return { url: b, docId: e };
                    };
                    a.destroyDownloadUserData = function (a) {
                        Module._free(Module.getValue(a, "i8*"));
                        Module._free(a);
                    };
                    l.downloadDataMap = {};
                    Module.customHeadersMap = {};
                    Module.withCredentials = {};
                    a.GetDownloadData = function (a) {
                        if (a in downloadDataMap) return downloadDataMap[a];
                    };
                    a.DownloaderHint = function (a, b) {
                        var e = this,
                            k = Module.GetDoc(a),
                            c = downloadDataMap[k];
                        b.currentPage && (c.m_current_page = b.currentPage);
                        if (b.visiblePages) {
                            var d = b.visiblePages;
                            for (b = 0; b < d.length; ++b) ++d[b];
                            Object.keys(this.requirePageCallbacks).forEach(function (a) {
                                e.requirePageCallbacks.hasOwnProperty(a) && d.push(parseInt(a, 10));
                            });
                            (b = Module.docs[a].chunkStorage) && b.keepVisibleChunks(c.downloader, d);
                            c = d.length;
                            a = Module.allocate(4 * c, "i8", Module.ALLOC_STACK);
                            for (b = 0; b < c; ++b) Module.setValue(a + 4 * b, d[b], "i32");
                            REX(Module._TRN_PDFDocDownloadPages(k, a, c, 1, 0));
                        }
                    };
                    a.RequirePage = function (a, b) {
                        return new Promise(function (e, k) {
                            k = Module.GetDoc(a);
                            var c = downloadDataMap[k];
                            !c || c.m_finished_download || c.m_pages[b]
                                ? e()
                                : (b in c.requirePageCallbacks ? c.requirePageCallbacks[b].push(e) : (c.requirePageCallbacks[b] = [e]),
                                    (e = Module.allocate(4, "i8", Module.ALLOC_STACK)),
                                    Module.setValue(e, b, "i32"),
                                    Module._TRN_PDFDocDownloadPages(k, e, 1, 0, 0));
                        });
                    };
                    a.IsLinearizationValid = function (a) {
                        a = Module.GetDoc(a);
                        if ((a = downloadDataMap[a])) {
                            var e = Module.allocate(4, "i8", Module.ALLOC_STACK);
                            REX(Module._TRN_DownloaderIsLinearizationValid(a.downloader, e));
                            return 0 !== Module.getValue(e, "i8");
                        }
                        return !1;
                    };
                    a.ShouldRunRender = function (a, b) {
                        a = Module.GetDoc(a);
                        return (a = downloadDataMap[a]) ? (a.m_finished_download ? !0 : a.m_pages[b]) : !0;
                    };
                    a.createCallbacksStruct = function (a) {
                        if (!g) {
                            var e = function (a) {
                                return function (e) {
                                    var b = arguments;
                                    e in downloadDataMap
                                        ? a.apply(this, b)
                                        : d(function () {
                                            e in downloadDataMap && a.apply(this, b);
                                        }, 0);
                                };
                            };
                            g = {
                                downloadProc: Module.addFunction(function (a, e, b, c, k) {
                                    c = Module.extractDownloadUserData(c);
                                    var d = c.docId;
                                    n(c.url, e, b, Module.customHeadersMap[d], Module.withCredentials[d]).then(function (c) {
                                        d in Module.docs && Module.docs[d].chunkStorage && Module.docs[d].chunkStorage.writeChunk(c, e);
                                        Module._TRN_DownloadComplete(0, e, b, a);
                                    });
                                }, "viiiii"),
                                notifyUpdatePage: Module.addFunction(
                                    e(function (a, e, b, c) {
                                        var k = downloadDataMap[a];
                                        k.m_pages[e] = !0;
                                        var d = k.eventPageArray;
                                        if (e in k.requirePageCallbacks) for (b = k.requirePageCallbacks[e], c = 0; c < b.length; ++c) b[c]();
                                        k.timeout
                                            ? d.push(e)
                                            : ((d = k.eventPageArray = [e]),
                                                (k.timeout = setTimeout(function () {
                                                    Module.postPagesUpdatedEvent(a, k.m_id, d);
                                                    k.timeout = null;
                                                }, 100)));
                                    }),
                                    "viiii"
                                ),
                                notifyUpdateOutline: Module.addFunction(
                                    e(function (a, e) {
                                        a = downloadDataMap[a];
                                        a.m_has_outline || ((a.m_has_outline = !0), Module.postEvent(a.m_id, "bookmarksUpdated", {}));
                                    }),
                                    "vii"
                                ),
                                notifyUpdateNamedDests: Module.addFunction(
                                    e(function (a, e) {
                                        a = downloadDataMap[a];
                                        a.m_has_named_dests || (a.m_has_named_dests = !0);
                                    }),
                                    "vii"
                                ),
                                notifyUpdateThumb: Module.addFunction(
                                    e(function (a, e) { }),
                                    "vii"
                                ),
                                notifyFinishedDownload: Module.addFunction(
                                    e(function (a, e) {
                                        a = downloadDataMap[a];
                                        a.m_finished_download || ((a.m_finished_download = !0), Module.postEvent(a.m_id, "documentComplete", {}));
                                    }),
                                    "vii"
                                ),
                                notifyDocumentError: Module.addFunction(function (a, e) { }, "vii"),
                                getCurrentPage: Module.addFunction(function (a, e) {
                                    return downloadDataMap[a].m_current_page;
                                }, "iii"),
                            };
                        }
                        e = Module.allocate(40, "i8", Module.ALLOC_STACK);
                        Module.setValue(e, g.downloadProc, "i8*");
                        Module.setValue(e + 4, a, "i8*");
                        Module.setValue(e + 8, g.notifyUpdatePage, "i8*");
                        Module.setValue(e + 12, g.notifyUpdateOutline, "i8*");
                        Module.setValue(e + 16, g.notifyUpdateNamedDests, "i8*");
                        Module.setValue(e + 20, g.notifyUpdateThumb, "i8*");
                        Module.setValue(e + 24, g.notifyFinishedDownload, "i8*");
                        Module.setValue(e + 28, g.notifyDocumentError, "i8*");
                        Module.setValue(e + 32, g.getCurrentPage, "i8*");
                        Module.setValue(e + 36, 0, "i8*");
                        return e;
                    };
                    a.CreateDocDownloaderBackend = function (a, b, c) {
                        var e = a.url,
                            k = a.size,
                            d = a.customHeaders,
                            f = a.withCredentials;
                        d && (Module.customHeadersMap[c] = d);
                        f && (Module.withCredentials[c] = f);
                        var q = a.downloadData ? new H(a.downloadData, e, c, d, f) : new H(a.size, e, c, d, f);
                        var g = Module.createCallbacksStruct(q.downloadUserData),
                            h = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        Module.MakeDev(c);
                        a.chunkStorage ? (e = new u(a.chunkStorage)) : ((a = new Int8Array(new ArrayBuffer(Math.ceil((a.size + 1048576 - 1) / 1048576 / 8)))), (e = new u(e, k, d, f, a)));
                        Module.docs[c] = { chunkStorage: e };
                        REX(Module._TRN_DownloaderCreate(g, k, Module.GetUStringFromJSString(c), h));
                        q.downloader = Module.getValue(h, "i8*");
                        if ((k = Module._TRN_PDFDocCreateFromFilter(q.downloader, b))) Module._TRN_FilterDestroy(q.downloader), REX(k);
                        b = Module.getValue(b, "i8*");
                        Module.docs[c].ptr = b;
                        Module.PDFDocInitSecurityHandler(b) && REX(Module._TRN_PDFDocDownloaderInitialize(b, q.downloader));
                        downloadDataMap[b] = q;
                    };
                    a.CreateDocBackend = function (e, b) {
                        var k = e.value,
                            d = e.extension,
                            g = e.type,
                            h = Module.allocate(4, "i8", Module.ALLOC_STACK),
                            m = Module.stackSave();
                        try {
                            if (k)
                                if ("ptr" === g) Module.docs[b] = { ptr: k };
                                else {
                                    var l = "object" === f(k) && k.url;
                                    g = d && "pdf" !== d;
                                    if (l) a.CreateDocDownloaderBackend(k, h, b);
                                    else {
                                        var n = k instanceof ArrayBuffer;
                                        l = n ? "buffer" : "file";
                                        if (n && ((k = new Uint8Array(k)), 10485760 > k.length + c && !g)) {
                                            c += k.length;
                                            var u = k.length,
                                                r = Module._malloc(k.length);
                                            Module.HEAPU8.set(k, r);
                                            REX(Module._TRN_PDFDocCreateFromBuffer(r, u, h));
                                            var p = Module.getValue(h, "i8*");
                                            Module.docs[b] = { ptr: p, bufPtr: r, bufPtrSize: u, ownership: !0 };
                                            Module.docs[b].extension = d;
                                            return;
                                        }
                                        Module.MakeDev(b);
                                        n = {};
                                        n[l] = k;
                                        Module.docs[b] = n;
                                        if (g) {
                                            if (e.pageSizes && e.pageSizes.length) var w = e.pageSizes[0];
                                            else e.defaultPageSize && (w = e.defaultPageSize);
                                            var t = Module.GetUStringFromJSString(b);
                                            REX(Module._TRN_PDFDocCreate(h));
                                            p = Module.getValue(h, "i8*");
                                            var z = Module.ImageCreateFromFile(p, t);
                                            Module.InsertImageIntoDoc(p, z, w);
                                        } else {
                                            var A = Module.allocate(Module.intArrayFromString(b), "i8", Module.ALLOC_STACK);
                                            REX(Module._TRN_PDFDocCreateFromFilePath(A, h));
                                            p = Module.getValue(h, "i8*");
                                        }
                                        Module.docs[b].extension = d;
                                        Module.docs[b].ptr = p;
                                    }
                                }
                            else REX(Module._TRN_PDFDocCreate(h)), (p = Module.getValue(h, "i8*")), (Module.docs[b] = { ptr: p }), (Module.docs[b].extension = d);
                        } finally {
                            Module.stackRestore(m);
                        }
                    };
                    a.ChangeDocBackend = function (a, k) {
                        var e = Module.docs[a];
                        e ? (e.bufPtr && e.ownership && (Module._free(e.bufPtr), (c -= e.bufPtrSize)), delete Module.docs[a]) : Object(b.d)("Trying to delete document ".concat(a, " that does not exist."));
                        Module.docs[a] = k;
                    };
                    a.DeleteDoc = function (a) {
                        var e = Module.docs[a];
                        e
                            ? (e.ptr && (e.ptr in downloadDataMap && (clearTimeout(downloadDataMap[e.ptr].timeout), downloadDataMap[e.ptr].destroyUserData(), delete downloadDataMap[e.ptr]), Module.PDFDocDestroy(e.ptr)),
                                e.bufPtr && e.ownership && (Module._free(e.bufPtr), (c -= e.bufPtrSize)),
                                delete Module.docs[a])
                            : Object(b.d)("Trying to delete document ".concat(a, " that does not exist."));
                    };
                    a.CreateDoc = function (e, b) {
                        if ("id" === e.type) return Module.docPtrStringToIdMap[e.value];
                        if (!b) {
                            do b = (++m).toString();
                            while (b in Module.docs);
                        }
                        Module.hasBufOwnership = !0;
                        a.CreateDocBackend(e, b);
                        return b;
                    };
                    a.CreateEmptyDoc = function () {
                        var a = (++m).toString(),
                            b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_PDFDocCreate(b));
                        b = Module.getValue(b, "i8*");
                        Module.docs[a] = { ptr: b };
                        return a;
                    };
                    a.PDFDocCreateFromLayoutEls = function (a) {
                        var b = new Uint8Array(a);
                        a = Module._malloc(b.length);
                        var e = Module.stackSave(),
                            c = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        Module.HEAPU8.set(b, a);
                        b = Module._TRN_PDFDocCreateFromLayoutEls(a, b.length, c);
                        c = Module.getValue(c, "i8*");
                        Module._free(a);
                        Module.stackRestore(e);
                        REX(b);
                        return c;
                    };
                    a.GetPageCount = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_PDFDocGetPageCount(a, b));
                        return Module.getValue(b, "i8*");
                    };
                    a.GetPage = function (a, b) {
                        var e = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_PDFDocGetPage(a, b, e));
                        a = Module.getValue(e, "i8*");
                        Module.PageIsValid(a) || THROW("Trying to access page that doesn't exist at index ".concat(b));
                        return a;
                    };
                    a.PageGetPageWidth = function (a) {
                        var b = Module.allocate(8, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_PageGetPageWidth(a, 1, b));
                        return Module.getValue(b, "double");
                    };
                    a.PageGetPageHeight = function (a) {
                        var b = Module.allocate(8, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_PageGetPageHeight(a, 1, b));
                        return Module.getValue(b, "double");
                    };
                    a.PageGetDefaultMatrix = function (a, b) {
                        var e = Module.allocate(48, "i8", Module.ALLOC_STACK);
                        b || (b = 0);
                        REX(Module._TRN_PageGetDefaultMatrix(a, !0, 1, b, e));
                        return e;
                    };
                    a.GetMatrixAsArray = function (a) {
                        for (var b = [], e = 0; 6 > e; ++e) b[e] = Module.getValue(a + 8 * e, "double");
                        return b;
                    };
                    a.PageGetPageInfo = function (a) {
                        var b = Module.allocate(48, "i8", Module.ALLOC_STACK),
                            e = Module.allocate(8, "i8", Module.ALLOC_STACK),
                            c = Module.allocate(8, "i8", Module.ALLOC_STACK),
                            d = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_PageGetPageInfo(a, !0, 1, 0, e, c, b, d));
                        return { rotation: Module.getValue(d, "i8*"), width: Module.getValue(e, "double"), height: Module.getValue(c, "double"), matrix: Module.GetMatrixAsArray(b) };
                    };
                    a.GetUStringFromJSString = function (a, b) {
                        var e = Module.allocate(4, "i8", Module.ALLOC_STACK),
                            c = 2 * (a.length + 1),
                            d = Module.allocate(c, "i8", b ? Module.ALLOC_NORMAL : Module.ALLOC_STACK);
                        Module.stringToUTF16(a, d, c);
                        a = Module._TRN_UStringCreateFromString(d, e);
                        b && Module._free(d);
                        REX(a);
                        return Module.getValue(e, "i8*");
                    };
                    a.GetJSStringFromUString = function (a) {
                        var b = Module.allocate(4, "i16*", Module.ALLOC_STACK);
                        REX(Module._TRN_UStringCStr(a, b));
                        return Module.UTF16ToString(Module.getValue(b, "i16*"));
                    };
                    a.GetJSStringFromCString = function (a) {
                        return Module.UTF8ToString(a);
                    };
                    a.PDFNetSetResourceData = function (a) {
                        Module.res_ptr = Module._malloc(a.length);
                        Module.HEAPU8.set(a, Module.res_ptr);
                        REX(Module._TRN_PDFNetSetResourceData(Module.res_ptr, a.length));
                        Module.res_ptr_size = a.length;
                    };
                    a.PDFDocDestroy = function (a) {
                        REX(Module._TRN_PDFDocDestroy(a));
                    };
                    a.VectorGetSize = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_VectorGetSize(a, b));
                        return Module.getValue(b, "i32");
                    };
                    a.VectorGetAt = function (a, b) {
                        var e = Module.allocate(1, "i8*", Module.ALLOC_STACK);
                        REX(Module._TRN_VectorGetAt(a, b, e));
                        return Module.getValue(e, "i8*");
                    };
                    a.VectorDestroy = function (a) {
                        REX(Module._TRN_VectorDestroy(a));
                    };
                    a.PDFRasterizerCreate = function () {
                        var a = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_PDFRasterizerCreate(0, a));
                        return Module.getValue(a, "i8*");
                    };
                    a.ExtractSeparationData = function (a) {
                        var b = Module.getValue(a, "i8*"),
                            e = Module.getValue(a + 4, "i32"),
                            c = Module.getValue(a + 8, "i8*"),
                            d = Module.HEAPU8[a + 12],
                            f = Module.HEAPU8[a + 13],
                            g = Module.HEAPU8[a + 14];
                        a = Module.HEAPU8[a + 15];
                        var h = new Uint8Array(e);
                        h.set(Module.HEAPU8.subarray(b, b + e));
                        b = Module.GetJSStringFromUString(c);
                        return { color: [d, f, g, a], data: h.buffer, name: b };
                    };
                    a.PDFRasterizerRasterizeSeparations = function (a, b, c, d, f, g, h) {
                        var e = Module.allocate(8, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_PDFRasterizerRasterizeSeparations(a, b, c, d, f, g, h, e));
                        a = Module.getValue(e, "i8*");
                        b = Module.VectorGetSize(a);
                        c = Array(b);
                        for (d = 0; d < b; ++d) (f = Module.VectorGetAt(a, d)), (c[d] = Module.ExtractSeparationData(f));
                        Module.VectorDestroy(a);
                        return c;
                    };
                    a.PageGetRotation = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_PageGetRotation(a, b));
                        return Module.getValue(b, "i8*");
                    };
                    a.PageGetId = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_PageGetSDFObj(a, b));
                        b = Module.getValue(b, "i8*");
                        a = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_ObjGetObjNum(b, a));
                        a = Module.getValue(a, "i32");
                        var e = Module.allocate(2, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_ObjGetGenNum(b, e));
                        e = Module.getValue(e, "i16");
                        return "".concat(a, "-").concat(e);
                    };
                    a.PageSetRotation = function (a, b) {
                        REX(Module._TRN_PageSetRotation(a, b));
                    };
                    a.GetDefaultMatrixBox = function (a, b, c) {
                        var e = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_PageGetRotation(a, e));
                        a = (Module.getValue(e, "i32") + c) % 4;
                        c = Module.allocate(48, "i8", Module.ALLOC_STACK);
                        switch (a) {
                            case 0:
                                return REX(Module._TRN_Matrix2DSet(c, 1, 0, 0, -1, -b.x1, b.y2)), c;
                            case 1:
                                return REX(Module._TRN_Matrix2DSet(c, 0, 1, 1, 0, -b.y1, -b.x1)), c;
                            case 2:
                                return REX(Module._TRN_Matrix2DSet(c, -1, 0, 0, 1, b.x2, -b.y1)), c;
                            case 3:
                                return REX(Module._TRN_Matrix2DSet(c, 0, -1, -1, 0, b.y2, b.x2)), c;
                        }
                        throw Error("Yikes, we don't support that rotation type");
                    };
                    a.Matrix2DMultBBox = function (a, b) {
                        var e = Module.allocate(8, "i8", Module.ALLOC_STACK),
                            c = Module.allocate(8, "i8", Module.ALLOC_STACK);
                        Module.setValue(e, b.x1, "double");
                        Module.setValue(c, b.y1, "double");
                        REX(Module._TRN_Matrix2DMult(a, e, c));
                        b.x1 = Module.getValue(e, "double");
                        b.y1 = Module.getValue(c, "double");
                        Module.setValue(e, b.x2, "double");
                        Module.setValue(c, b.y2, "double");
                        REX(Module._TRN_Matrix2DMult(a, e, c));
                        b.x2 = Module.getValue(e, "double");
                        b.y2 = Module.getValue(c, "double");
                        return b;
                    };
                    a.Matrix2DMult = function (a, b) {
                        var e = Module.allocate(8, "i8", Module.ALLOC_STACK),
                            c = Module.allocate(8, "i8", Module.ALLOC_STACK);
                        Module.setValue(e, b.x, "double");
                        Module.setValue(c, b.y, "double");
                        REX(Module._TRN_Matrix2DMult(a, e, c));
                        b.x = Module.getValue(e, "double");
                        b.y = Module.getValue(c, "double");
                        return b;
                    };
                    a.Matrix2DConcat = function (a, b) {
                        var e = Module.getValue(b, "double"),
                            c = Module.getValue(b + 8, "double"),
                            d = Module.getValue(b + 16, "double"),
                            k = Module.getValue(b + 24, "double"),
                            f = Module.getValue(b + 32, "double");
                        b = Module.getValue(b + 40, "double");
                        REX(Module._TRN_Matrix2DConcat(a, e, c, d, k, f, b));
                    };
                    a.Matrix2DSet = function (a, b, c, d, f, g, h) {
                        REX(Module._TRN_Matrix2DSet(a, b, c, d, f, g, h));
                    };
                    a.IteratorHasNext = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_IteratorHasNext(a, b));
                        return 0 !== Module.getValue(b, "i8");
                    };
                    a.IteratorCurrent = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_IteratorCurrent(a, b));
                        return Module.getValue(Module.getValue(b, "i8*"), "i8*");
                    };
                    a.IteratorNext = function (a) {
                        REX(Module._TRN_IteratorNext(a));
                    };
                    a.PageGetNumAnnots = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_PageGetNumAnnots(a, b));
                        return Module.getValue(b, "i32");
                    };
                    a.PageGetAnnot = function (a, b) {
                        var e = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_PageGetAnnot(a, b, e));
                        return Module.getValue(e, "i8*");
                    };
                    a.PageAnnotRemove = function (a, b) {
                        REX(Module._TRN_PageAnnotRemoveByIndex(a, b));
                    };
                    a.AnnotGetType = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_AnnotGetType(a, b));
                        return Module.getValue(b, "i32");
                    };
                    a.AnnotHasAppearance = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_AnnotGetAppearance(a, 0, 0, b));
                        return 0 !== Module.getValue(b, "i8*");
                    };
                    a.AnnotRefreshAppearance = function (a) {
                        REX(Module._TRN_AnnotRefreshAppearance(a));
                    };
                    a.ObjErase = function (a, b) {
                        b = Module.allocate(Module.intArrayFromString(b), "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_ObjEraseFromKey(a, b));
                    };
                    a.GetJSDoubleArrFromCore = function (a, b) {
                        for (var c = Array(b), e = 0; e < b; ++e) (c[e] = Module.getValue(a, "double")), (a += 8);
                        return c;
                    };
                    a.GetJSIntArrayFromCore = function (a, b) {
                        for (var c = Array(b), e = 0; e < b; ++e) (c[e] = Module.getValue(a, "i32")), (a += 4);
                        return c;
                    };
                    a.BookmarkIsValid = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_BookmarkIsValid(a, b));
                        return 0 !== Module.getValue(b, "i8");
                    };
                    a.BookmarkGetNext = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_BookmarkGetNext(a, b));
                        return Module.getValue(b, "i8*");
                    };
                    a.BookmarkGetFirstChild = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_BookmarkGetFirstChild(a, b));
                        return Module.getValue(b, "i8*");
                    };
                    a.BookmarkHasChildren = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_BookmarkHasChildren(a, b));
                        return 0 !== Module.getValue(b, "i8");
                    };
                    a.BookmarkGetAction = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_BookmarkGetAction(a, b));
                        return Module.getValue(b, "i8*");
                    };
                    a.BookmarkGetTitle = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_BookmarkGetTitle(a, b));
                        a = Module.getValue(b, "i8*");
                        return Module.GetJSStringFromUString(a);
                    };
                    a.ActionIsValid = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_ActionIsValid(a, b));
                        return 0 !== Module.getValue(b, "i8");
                    };
                    a.ActionGetType = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_ActionGetType(a, b));
                        return Module.getValue(b, "i32");
                    };
                    a.ActionGetDest = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_ActionGetDest(a, b));
                        return Module.getValue(b, "i8*");
                    };
                    a.DestinationIsValid = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_DestinationIsValid(a, b));
                        return 0 !== Module.getValue(b, "i8");
                    };
                    a.DestinationGetPage = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_DestinationGetPage(a, b));
                        return Module.getValue(b, "i8*");
                    };
                    a.PageIsValid = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_PageIsValid(a, b));
                        return 0 !== Module.getValue(b, "i8");
                    };
                    a.PageGetIndex = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_PageGetIndex(a, b));
                        return Module.getValue(b, "i32");
                    };
                    a.ObjGetAsPDFText = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_ObjGetAsPDFText(a, b));
                        a = Module.getValue(b, "i8*");
                        return Module.GetJSStringFromUString(a);
                    };
                    a.ObjFindObj = function (a, b) {
                        var c = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        b = Module.allocate(Module.intArrayFromString(b), "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_ObjFindObj(a, b, c));
                        return Module.getValue(c, "i8*");
                    };
                    a.PDFDocGetFirstBookmark = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_PDFDocGetFirstBookmark(a, b));
                        return Module.getValue(b, "i8*");
                    };
                    a.DestinationGetExplicitDestObj = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_DestinationGetExplicitDestObj(a, b));
                        return Module.getValue(b, "i8*");
                    };
                    a.DestinationGetFitType = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_DestinationGetFitType(a, b));
                        return Module.getValue(b, "i32");
                    };
                    a.ObjIsNumber = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_ObjIsNumber(a, b));
                        return 0 !== Module.getValue(b, "i8");
                    };
                    a.ObjGetNumber = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_ObjGetNumber(a, b));
                        return Module.getValue(b, "double");
                    };
                    a.PDFDocGetRoot = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_PDFDocGetRoot(a, b));
                        return Module.getValue(b, "i8*");
                    };
                    a.ObjPutName = function (a, b, c) {
                        b = Module.allocate(Module.intArrayFromString(b), "i8", Module.ALLOC_STACK);
                        c = Module.allocate(Module.intArrayFromString(c), "i8", Module.ALLOC_STACK);
                        var e = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_ObjPutName(a, b, c, e));
                        return Module.getValue(e, "i8*");
                    };
                    a.ObjPutDict = function (a, b) {
                        b = Module.allocate(Module.intArrayFromString(b), "i8", Module.ALLOC_STACK);
                        var c = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_ObjPutDict(a, b, c));
                        return Module.getValue(c, "i8*");
                    };
                    a.ObjPutString = function (a, b, c) {
                        b = Module.allocate(Module.intArrayFromString(b), "i8", Module.ALLOC_STACK);
                        c = Module.allocate(Module.intArrayFromString(c), "i8", Module.ALLOC_STACK);
                        var e = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_ObjPutString(a, b, c, e));
                        return Module.getValue(e, "i8*");
                    };
                    a.ObjPut = function (a, b, c) {
                        b = Module.allocate(Module.intArrayFromString(b), "i8", Module.ALLOC_STACK);
                        var e = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_ObjPut(a, b, c, e));
                        return Module.getValue(e, "i8*");
                    };
                    a.ObjGetAt = function (a, b) {
                        var c = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_ObjGetAt(a, b, c));
                        return Module.getValue(c, "i8*");
                    };
                    a.Matrix2DInverse = function (a) {
                        var b = Module.allocate(48, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_Matrix2DInverse(a, b));
                        return b;
                    };
                    a.PDFDocInitSecurityHandler = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_PDFDocInitSecurityHandler(a, 0, b));
                        return 0 !== Module.getValue(b, "i8");
                    };
                    a.PDFDocInitStdSecurityHandler = function (a, b) {
                        var c = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_PDFDocInitStdSecurityHandlerUString(a, Module.GetUStringFromJSString(b), c));
                        return 0 !== Module.getValue(c, "i8");
                    };
                    a.PDFDocInsertPages = function (a, b, c, d, f) {
                        REX(Module._TRN_PDFDocInsertPageSet(a, b, c, d, f ? 1 : 0, 0));
                    };
                    a.PDFDocMovePages = function (a, b, c) {
                        REX(Module._TRN_PDFDocMovePageSet(a, b, a, c, 0, 0));
                    };
                    a.PDFDocGetPageIterator = function (a, b) {
                        var c = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_PDFDocGetPageIterator(a, b, c));
                        return Module.getValue(c, "i8*");
                    };
                    a.PDFDocPageRemove = function (a, b) {
                        REX(Module._TRN_PDFDocPageRemove(a, b));
                    };
                    a.PDFDocPageCreate = function (a, b, c) {
                        var e = Module.allocate(40, "i8", Module.ALLOC_STACK);
                        Module.setValue(e, 0, "double");
                        Module.setValue(e + 8, 0, "double");
                        Module.setValue(e + 16, b, "double");
                        Module.setValue(e + 24, c, "double");
                        Module.setValue(e + 32, 0, "double");
                        b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_PDFDocPageCreate(a, e, b));
                        return Module.getValue(b, "i8*");
                    };
                    a.PDFDocPageInsert = function (a, b, c) {
                        REX(Module._TRN_PDFDocPageInsert(a, b, c));
                    };
                    a.PageSetCreate = function () {
                        var a = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_PageSetCreate(a));
                        return Module.getValue(a, "i8*");
                    };
                    a.PageSetCreateRange = function (a, b) {
                        var c = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_PageSetCreateRange(c, a, b));
                        return Module.getValue(c, "i8*");
                    };
                    a.PageSetAddPage = function (a, b) {
                        REX(Module._TRN_PageSetAddPage(a, b));
                    };
                    a.ElementBuilderCreate = function () {
                        var a = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_ElementBuilderCreate(a));
                        return Module.getValue(a, "i8*");
                    };
                    a.ElementBuilderDestroy = function (a) {
                        REX(Module._TRN_ElementBuilderDestroy(a));
                    };
                    a.ElementBuilderCreateImage = function (a, b, c, d, f, g) {
                        var e = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_ElementBuilderCreateImageScaled(a, b, c, d, f, g, e));
                        return Module.getValue(e, "i8*");
                    };
                    a.ElementWriterCreate = function () {
                        var a = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_ElementWriterCreate(a));
                        return Module.getValue(a, "i8*");
                    };
                    a.ElementWriterDestroy = function (a) {
                        REX(Module._TRN_ElementWriterDestroy(a));
                    };
                    a.ElementWriterBegin = function (a, b) {
                        REX(Module._TRN_ElementWriterBeginOnPage(a, b, 1));
                    };
                    a.ElementWriterWritePlacedElement = function (a, b) {
                        REX(Module._TRN_ElementWriterWritePlacedElement(a, b));
                    };
                    a.ElementWriterEnd = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_ElementWriterEnd(a, b));
                    };
                    a.ImageGetImageWidth = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_ImageGetImageWidth(a, b));
                        return Module.getValue(b, "i32");
                    };
                    a.ImageGetImageHeight = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_ImageGetImageHeight(a, b));
                        return Module.getValue(b, "i32");
                    };
                    a.ImageCreateFromMemory2 = function (a, b, c) {
                        var d = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_ImageCreateFromMemory2(a, b, c, 0, d));
                        return Module.getValue(d, "i8*");
                    };
                    a.ImageCreateFromFile = function (a, b) {
                        var c = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_ImageCreateFromFile(a, b, 0, c));
                        return Module.getValue(c, "i8*");
                    };
                    a.PDFDocPagePushBack = function (a, b) {
                        REX(Module._TRN_PDFDocPagePushBack(a, b));
                    };
                    a.PDFDocHasOC = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_PDFDocHasOC(a, b));
                        return 0 !== Module.getValue(b, "i8");
                    };
                    a.PDFDocGetOCGConfig = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_PDFDocGetOCGConfig(a, b));
                        return Module.getValue(b, "i8*");
                    };
                    a.OCGContextCreate = function (a) {
                        var b = Module.allocate(4, "i8", Module.ALLOC_STACK);
                        REX(Module._TRN_OCGContextCreateFromConfig(a, b));
                        return Module.getValue(b, "i8*");
                    };
                    a.UStringDestroy = function (a) {
                        REX(Module._TRN_UStringDestroy(a));
                    };
                    a.PDFDocFDFUpdate = function (a, b, c) {
                        if (c) {
                            for (var d = Object.keys(c), e = d.length, f = Module._malloc(8 * e), k = 0; k < e; ++k) {
                                var g = 8 * k,
                                    h = d[k],
                                    m = Module.GetDoc(c[h]);
                                h = Module.GetUStringFromJSString(h);
                                Module.setValue(f + g, m, "i8*");
                                Module.setValue(f + g + 4, h, "i8*");
                            }
                            REX(Module._TRN_PDFDocFDFUpdateAppearanceDocs(a, b, f, e));
                        } else REX(Module._TRN_PDFDocFDFUpdate(a, b));
                    };
                    a.FDFDocDestroy = function (a) {
                        REX(Module._TRN_FDFDocDestroy(a));
                    };
                })(l.Module);
            }.call(this, g(8).setImmediate));
        },
        function (d, l, g) {
            function f(d) {
                f =
                    "function" === typeof Symbol && "symbol" === typeof Symbol.iterator
                        ? function (b) {
                            return typeof b;
                        }
                        : function (b) {
                            return b && "function" === typeof Symbol && b.constructor === Symbol && b !== Symbol.prototype ? "symbol" : typeof b;
                        };
                return f(d);
            }
            (function (d) {
                d.SetupPDFNetFunctions = function (b) {
                    Module._IB_ = [];
                    for (
                        var g = function A(a) {
                            if ("object" === f(a) && null !== a)
                                if ("undefined" !== typeof a.byteLength) {
                                    var b = Module._IB_.length;
                                    Module._IB_[b] = new Uint8Array(a);
                                    a = { handle: b, isArrayBufferRef: !0 };
                                } else
                                    Object.keys(a).forEach(function (b) {
                                        a.hasOwnProperty(b) && (a[b] = A(a[b]));
                                    });
                            return a;
                        },
                        a = function r(a) {
                            "object" === f(a) &&
                                null !== a &&
                                (a.buffer
                                    ? (a = a.buffer.slice(a.byteOffset, a.byteOffset + a.length))
                                    : a.isArrayBufferRef
                                        ? (a = Module._IB_[a.handle].buffer)
                                        : Object.keys(a).forEach(function (b) {
                                            a.hasOwnProperty(b) && (a[b] = r(a[b]));
                                        }));
                            return a;
                        },
                        c = Module._TRN_EMSCreateSharedWorkerInstance(),
                        h,
                        l = Module._TRN_EMSWorkerInstanceGetFunctionIterator(c),
                        x = function (b, d) {
                            return new Promise(function (f, h) {
                                b = g(b);
                                var e = b.docId;
                                e = e ? Module.GetDoc(e) : 0;
                                (e = Module.EMSCallSharedFunction(c, d, e)) ? h({ type: "InvalidPDF", message: Module.GetErrToString(e) }) : ((h = Module.EMSGetLastResponse(c)), (h = a(h)), f(h));
                            });
                        };
                        (h = Module._TRN_EMSFunctionIteratorGetNextCommandName(l));

                    )
                        (h = Module.GetJSStringFromCString(h)), d.queue.onAsync(h, x);
                    Module._TRN_EMSFunctionIteratorDestroy(l);
                    if (Module._TRN_EMSCreatePDFNetWorkerInstance) {
                        var n = {};
                        l = function (a, c) {
                            b.on(a, c);
                            n[a] = !0;
                        };
                        Module.docPtrStringToIdMap = {};
                        var w = function (a) {
                            if (a in Module.docPtrStringToIdMap) return Module.docPtrStringToIdMap[a];
                            throw Error("Couldn't find document ".concat(a));
                        };
                        d.queue.onAsync("PDFDoc.RequirePage", function (a) {
                            return Module.RequirePage(w(a.docId), a.pageNum);
                        });
                        l("pdfDocCreateFromBuffer", function (a) {
                            a = Module.CreateDoc({ type: "array", value: a.buf });
                            var b = Module.GetDoc(a).toString(16);
                            Module.docPtrStringToIdMap[b] = a;
                            return b;
                        });
                        l("PDFDoc.destroy", function (a) {
                            a = w(a.auto_dealloc_obj);
                            Module.DeleteDoc(a);
                        });
                        l("PDFDoc.saveMemoryBuffer", function (a) {
                            var b = w(a.doc);
                            return Module.SaveHelper(Module.GetDoc(b), b, a.flags).slice(0);
                        });
                        l("pdfDocCreate", function () {
                            var a = Module.CreateDoc({ type: "new" }),
                                b = Module.GetDoc(a).toString(16);
                            Module.docPtrStringToIdMap[b] = a;
                            return b;
                        });
                        l("GetPDFDoc", function (a) {
                            a = a.docId;
                            var b = Module.GetDoc(a).toString(16);
                            Module.docPtrStringToIdMap[b] = a;
                            return b;
                        });
                        l("ExtractPDFNetLayersContext", function (a) {
                            var b = a.layers;
                            a = Module.GetDoc(a.docId);
                            var c = 0;
                            b ? (c = Module.EMSCreateUpdatedLayersContext(a, b)) : Module.PDFDocHasOC(a) && ((b = Module.PDFDocGetOCGConfig(a)), (c = Module.OCGContextCreate(b)));
                            return c.toString(16);
                        });
                        var y = Module._TRN_EMSCreatePDFNetWorkerInstance();
                        l = Module._TRN_EMSWorkerInstanceGetFunctionIterator(y);
                        for (
                            x = function (b) {
                                return new Promise(function (c, d) {
                                    b = g(b);
                                    var f = Module.EMSCallPDFNetFunction(y, b);
                                    f ? d(Module.GetErrToString(f)) : ((d = Module.EMSGetLastResponse(y)), (d = a(d)), c(d));
                                });
                            };
                            (h = Module._TRN_EMSFunctionIteratorGetNextCommandName(l));

                        )
                            if (((h = Module.GetJSStringFromCString(h)), !n[h])) b.onAsync(h, x);
                        Module._TRN_EMSFunctionIteratorDestroy(l);
                    }
                };
            })(self);
        },
        function (d, l, g) {
            d = g(7);
            var f = g.n(d),
                h = g(15),
                b = g(16),
                t = g(11),
                a = g(1),
                c = g(9);
            (function (d) {
                var g = null;
                d.basePath = "../";
                var l = function () {
                    var b = d.pdfWorkerPath || "";
                    d.workerBasePath && (d.basePath = d.workerBasePath);
                    var f = Object(c.a)(d.location.search),
                        g = "true" === f.isfull,
                        h = g ? "full/" : "lean/",
                        l = "0" === f.wasm;
                    Object(a.c)();
                    f.pdfWorkerPath && ((b = f.pdfWorkerPath), (d.basePath = "../"), !Object(t.b)() || l) && (b = "");
                    d.basePath = f.externalPath ? f.externalPath : d.basePath + "external/";
                    Object(t.a)("".concat(b + h, "PDFNetC"), { "Wasm.wasm": g ? 1e7 : 4e6, "Wasm.js.mem": 1e5, ".js.mem": 6e6, ".mem": g ? 2e6 : 3e5 }, l);
                };
                self.shouldResize || l();
                d.EmscriptenPDFManager = function () { };
                d.EmscriptenPDFManager.prototype = {
                    OnInitialized: function (b) {
                        Module.loaded
                            ? b()
                            : ((Module.initCb = function () {
                                b();
                            }),
                                Object(a.b)("worker", "PDFNet is not initialized yet!"));
                    },
                    NewDoc: function (a, b) {
                        return new Promise(function (c, d) {
                            try {
                                c(Module.loadDoc(a, b));
                            } catch (A) {
                                d(A);
                            }
                        });
                    },
                    Initialize: function (a, b, c, f) {
                        a && (Module.TOTAL_MEMORY = a);
                        Module.memoryInitializerPrefixURL = b;
                        Module.asmjsPrefix = c;
                        d.basePath = f;
                        l();
                    },
                    shouldRunRender: function (a) {
                        return Module.ShouldRunRender(a.docId, a.pageIndex + 1);
                    },
                };
                var m = new b.a("worker_processor", self);
                Object(a.a)(m);
                (function (b) {
                    function c(b) {
                        var c = b.data,
                            e = b.action;
                        var f = "GetCanvas" === e || "GetCanvasPartial" === e ? r.shouldRunRender(c) : !0;
                        if (f) {
                            g = b;
                            var h = b.asyncCallCapability;
                            Object(a.b)("Memory", "Worker running command: ".concat(e));
                            p.actionMap[e](c, b).then(
                                function (a) {
                                    "BeginOperation" !== g.action && (g = null);
                                    h.resolve(a);
                                },
                                function (a) {
                                    g = null;
                                    h.reject(a);
                                }
                            );
                        } else d.deferredQueue.queue(b), n();
                    }
                    function l(a) {
                        a.asyncCallCapability = createPromiseCapability();
                        g || p.length ? p.queue(a) : c(a);
                        return a.asyncCallCapability.promise;
                    }
                    function m(a) {
                        self.shouldResize && r.Initialize(a.options.workerHeapSize, a.options.pdfResourcePath, a.options.pdfAsmPath, a.options.parentUrl);
                        Module.chunkMax = a.options.chunkMax;
                        if (a.array instanceof Uint8Array) {
                            var c = 255 === a.array[0];
                            b.postMessageTransfers = c;
                            "response" in new XMLHttpRequest()
                                ? r.OnInitialized(function () {
                                    d.SetupPDFNetFunctions(b);
                                    k();
                                    b.send("test", { supportTypedArray: !0, supportTransfers: c });
                                })
                                : b.send("test", !1);
                        } else b.send("test", !1);
                    }
                    function n() {
                        h.a.setImmediate(function () {
                            if ((!g || "BeginOperation" !== g.action) && p.length && !g) {
                                var a = p.dequeue();
                                c(a);
                            }
                        });
                    }
                    var r = new d.EmscriptenPDFManager(),
                        p,
                        t = !1,
                        e = !1;
                    Module.workerMessageHandler = b;
                    var k = function () {
                        t ? e || (b.send("workerLoaded", {}), (e = !0)) : (t = !0);
                    };
                    r.OnInitialized(k);
                    (function () {
                        d.queue = p = new f.a({
                            strategy: f.a.ArrayStrategy,
                            comparator: function (a, b) {
                                return a.priority === b.priority ? a.callbackId - b.callbackId : b.priority - a.priority;
                            },
                        });
                        d.deferredQueue = new f.a({
                            strategy: f.a.ArrayStrategy,
                            comparator: function (a, b) {
                                return a.priority === b.priority ? a.callbackId - b.callbackId : b.priority - a.priority;
                            },
                        });
                        p.actionMap = {};
                        p.onAsync = function (a, c) {
                            b.onAsync(a, l);
                            p.actionMap[a] = c;
                        };
                    })();
                    b.on("test", m);
                    b.on("InitWorker", m);
                    var q = function (a) {
                        g && a(g) && (Module.cancelCurrent(), (g = null));
                        p.removeAllMatching(a, function (a) {
                            a.asyncCallCapability.reject({ type: "Cancelled", message: "Operation was cancelled due to a change affecting the loaded document." });
                        });
                    },
                        w = function (a) {
                            q(function (b) {
                                return b.data && b.data.docId === a;
                            });
                        };
                    b.on("UpdatePassword", function (a) {
                        return Module.UpdatePassword(a.docId, a.password);
                    });
                    b.on("LoadRes", function (a) {
                        Module.loadResources(a.array, a.l);
                        return {};
                    });
                    b.on("DownloaderHint", function (a) {
                        Module.DownloaderHint(a.docId, a.hint);
                    });
                    b.on("IsLinearized", function (a) {
                        return Module.IsLinearizationValid(a.docId);
                    });
                    b.onNextAsync(n);
                    p.onAsync("NewDoc", function (a) {
                        return r.NewDoc(a);
                    });
                    p.onAsync("GetCanvas", function (b) {
                        Object(a.b)("workerdetails", "Run GetCanvas PageIdx: ".concat(b.pageIndex, " Width: ").concat(b.width));
                        Object(a.b)("Memory", "loadCanvas with potential memory usage ".concat(b.width * b.height * 8));
                        return Module.loadCanvas(b.docId, b.pageIndex, b.width, b.height, b.rotation, null, b.layers, b.renderOptions);
                    });
                    p.onAsync("GetCanvasPartial", function (b) {
                        Object(a.b)("Memory", "GetCanvasPartial with potential memory usage ".concat(b.width * b.height * 8));
                        return Module.loadCanvas(b.docId, b.pageIndex, b.width, b.height, b.rotation, b.bbox, b.layers, b.renderOptions);
                    });
                    p.onAsync("SaveDoc", function (a) {
                        return Module.SaveDoc(a.docId, a.xfdfString, a.finishedWithDocument, a.printDocument, a.flags, a.watermarks, a.apdocs);
                    });
                    p.onAsync("SaveDocFromFixedElements", function (a) {
                        return Module.SaveDocFromFixedElements(a.bytes, a.xfdfString, a.flags, a.watermarks);
                    });
                    p.onAsync("MergeXFDF", function (a) {
                        return Module.MergeXFDF(a.docId, a.xfdf, a.apdocs);
                    });
                    p.onAsync("InsertPages", function (a) {
                        return Module.InsertPages(a.docId, a.doc, a.pageArray, a.destPos, a.insertBookmarks);
                    });
                    p.onAsync("MovePages", function (a) {
                        return Module.MovePages(a.docId, a.pageArray, a.destPos);
                    });
                    p.onAsync("RemovePages", function (a) {
                        return Module.RemovePages(a.docId, a.pageArray);
                    });
                    p.onAsync("RotatePages", function (a) {
                        return Module.RotatePages(a.docId, a.pageArray, a.rotation);
                    });
                    p.onAsync("ExtractPages", function (a) {
                        return Module.ExtractPages(a.docId, a.pageArray, a.xfdfString, a.watermarks, a.apdocs);
                    });
                    p.onAsync("CropPages", function (a) {
                        return Module.CropPages(a.docId, a.pageArray, a.topMargin, a.botMargin, a.leftMargin, a.rightMargin);
                    });
                    p.onAsync("InsertBlankPages", function (a) {
                        return Module.InsertBlankPages(a.docId, a.pageArray, a.width, a.height);
                    });
                    p.onAsync("BeginOperation", function () {
                        return Promise.resolve();
                    });
                    p.onAsync("RequirePage", function (a, b) {
                        return Module.RequirePage(a.docId, a.pageNum);
                    });
                    b.on("FinishOperation", function () {
                        if (g && "BeginOperation" === g.action) (g = null), n();
                        else throw { message: "Operation has not started." };
                    });
                    b.on("DeleteDocument", function (a) {
                        a = a.docId;
                        w(a);
                        Module.DeleteDoc(a);
                    });
                    b.on("GetCanvasProgressive", function (b) {
                        if (g && g.callbackId === b.callbackId) {
                            Object(a.b)("worker", "Progressive request in progress");
                            var c = Module.GetCurrentCanvasData(!0);
                        } else {
                            if (p.find({ priority: 0, callbackId: b.callbackId })) throw (Object(a.b)("worker", "Progressive request Queued"), { type: "Queued", message: "Rendering has not started yet." });
                            if (d.deferredQueue.find({ priority: 0, callbackId: b.callbackId })) throw (Object(a.b)("worker", "Progressive request Deferred"), { type: "Queued", message: "Rendering has not started yet." });
                        }
                        if (!c) throw (Object(a.b)("worker", "Progressive request invalid (render already complete)"), { type: "Unavailable", message: "Rendering is complete or was cancelled." });
                        return c;
                    });
                    b.on("actionCancel", function (b) {
                        g && g.callbackId === b.callbackId
                            ? (Object(a.b)("workerdetails", "Cancelled Current Operation"), Module.cancelCurrent() && ((g = null), n()))
                            : (Object(a.b)("workerdetails", "Cancelled queued operation"), p.remove({ priority: 0, callbackId: b.callbackId }), d.deferredQueue.remove({ priority: 0, callbackId: b.callbackId }));
                    });
                })(m);
            })("undefined" === typeof window ? self : window);
        },
    ]);
}.call(this || window));
