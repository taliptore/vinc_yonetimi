import 'package:flutter/foundation.dart';
import '../services/api_service.dart';

class AuthProvider with ChangeNotifier {
  final ApiService _api = ApiService();
  Map<String, dynamic>? _user;
  bool _loading = true;

  Map<String, dynamic>? get user => _user;
  bool get isLoggedIn => _user != null;
  bool get loading => _loading;

  Future<void> init() async {
    _loading = true;
    notifyListeners();
    _user = await _api.getUser();
    _loading = false;
    notifyListeners();
  }

  Future<void> login(String email, String password) async {
    _user = await _api.login(email, password);
    notifyListeners();
  }

  Future<void> logout() async {
    await _api.logout();
    _user = null;
    notifyListeners();
  }
}
