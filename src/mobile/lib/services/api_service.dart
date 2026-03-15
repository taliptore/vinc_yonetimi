import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class ApiService {
  static const String _baseUrl = 'http://10.0.2.2:5042/api/v1'; // Android emulator -> localhost
  // Gerçek cihaz için: 'http://BILGISAYAR_IP:5042/api/v1'
  final _storage = const FlutterSecureStorage();

  Future<String?> _getToken() => _storage.read(key: 'token');

  Future<Map<String, String>> _headers() async {
    final token = await _getToken();
    return {
      'Content-Type': 'application/json',
      if (token != null) 'Authorization': 'Bearer $token',
    };
  }

  Future<Map<String, dynamic>> login(String email, String password) async {
    final r = await http.post(
      Uri.parse('$_baseUrl/auth/login'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({'email': email, 'password': password}),
    );
    final body = jsonDecode(r.body) as Map<String, dynamic>?;
    if (r.statusCode != 200) throw Exception(body?['message'] ?? 'Giriş başarısız');
    await _storage.write(key: 'token', value: body!['token'] as String);
    await _storage.write(key: 'user', value: jsonEncode(body));
    return body;
  }

  Future<void> logout() async {
    await _storage.delete(key: 'token');
    await _storage.delete(key: 'user');
  }

  Future<Map<String, dynamic>?> getUser() async {
    final s = await _storage.read(key: 'user');
    if (s == null) return null;
    return jsonDecode(s) as Map<String, dynamic>;
  }

  Future<List<dynamic>> getJobs({DateTime? from, DateTime? to}) async {
    var url = '$_baseUrl/jobs';
    final q = <String>[];
    if (from != null) q.add('from=${from.toIso8601String()}');
    if (to != null) q.add('to=${to.toIso8601String()}');
    if (q.isNotEmpty) url += '?${q.join('&')}';
    final r = await http.get(Uri.parse(url), headers: await _headers());
    if (r.statusCode == 401) throw Exception('Unauthorized');
    if (r.statusCode != 200) throw Exception('İstek başarısız');
    return jsonDecode(r.body) as List<dynamic>;
  }

  Future<Map<String, dynamic>> getDashboard() async {
    final r = await http.get(Uri.parse('$_baseUrl/dashboard'), headers: await _headers());
    if (r.statusCode == 401) throw Exception('Unauthorized');
    if (r.statusCode != 200) throw Exception('Dashboard yüklenemedi');
    return jsonDecode(r.body) as Map<String, dynamic>;
  }

  Future<void> jobStart(int jobId, double lat, double lng) async {
    final r = await http.post(
      Uri.parse('$_baseUrl/jobs/$jobId/start'),
      headers: await _headers(),
      body: jsonEncode({'latitude': lat, 'longitude': lng}),
    );
    final body = r.body.isNotEmpty ? jsonDecode(r.body) as Map<String, dynamic>? : null;
    if (r.statusCode != 200) throw Exception(body?['message'] ?? 'İş başlatılamadı');
  }

  Future<Map<String, dynamic>> jobEnd(int jobId, double lat, double lng) async {
    final r = await http.post(
      Uri.parse('$_baseUrl/jobs/$jobId/end'),
      headers: await _headers(),
      body: jsonEncode({'latitude': lat, 'longitude': lng}),
    );
    final body = jsonDecode(r.body) as Map<String, dynamic>?;
    if (r.statusCode != 200) throw Exception(body?['message'] ?? 'İş bitirilemedi');
    return body ?? {};
  }

  Future<void> reportBreakdown(int craneId, String description) async {
    final r = await http.post(
      Uri.parse('$_baseUrl/breakdowns'),
      headers: await _headers(),
      body: jsonEncode({'craneId': craneId, 'description': description}),
    );
    if (r.statusCode != 200 && r.statusCode != 201) throw Exception('Arıza bildirimi gönderilemedi');
  }
}
